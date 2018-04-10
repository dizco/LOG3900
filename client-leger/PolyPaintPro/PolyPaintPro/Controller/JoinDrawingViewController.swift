//
//  JoinDrawingViewController.swift
//  PolyPaintPro
//
//  Created by Fred Habsfan on 2018-03-12.
//  Copyright © 2018 Les Pods c'est pour les lunchs. All rights reserved.
//

import Foundation
import UIKit
import PromiseKit

class JoinDrawingViewController: UIViewController, iCarouselDelegate, iCarouselDataSource {
    internal var connectionStatus = true
    private var myDrawingsList: [ExtendedDrawingModel] = []
    private var publicDrawingsList: [ExtendedDrawingModel] = []

    @IBOutlet var carouselView: iCarousel!
    @IBOutlet var carousel2View: iCarousel!

    override func viewDidLoad() {
        super.viewDidLoad()
        carouselView.type = .coverFlow
        carousel2View.type = .coverFlow
        loadOnlineDrawings()
    }

    override func didReceiveMemoryWarning() {
        super.didReceiveMemoryWarning()
    }

    func numberOfItems(in carousel: iCarousel) -> Int {
        if carousel == self.carouselView {
            return myDrawingsList.count
        }
        if carousel == self.carousel2View {
            return publicDrawingsList.count
        }
        return 0
    }

    func carousel(_ carousel: iCarousel, viewForItemAt index: Int, reusing view: UIView?) -> UIView {
        let cardsView = UIView(frame: CGRect(x: 0, y: 0, width: 300, height: 250))
        cardsView.backgroundColor = UIColor.white
        let thumbnailView = UIImageView(frame: CGRect(x: 0, y: 0, width: 300, height: 220))
        let nameLabel = UILabel(frame: CGRect(x: 0, y: 220, width: 200, height: 30))
        nameLabel.font = nameLabel.font.withSize(25)
        let protectionButton = UIButton(frame: CGRect(x: 200, y: 220, width: 50, height: 30))
        protectionButton.titleLabel?.font = UIFont.systemFont(ofSize: 25)
        let visibilityLabel = UILabel(frame: CGRect(x: 250, y: 220, width: 50, height: 30))
        visibilityLabel.font = visibilityLabel.font.withSize(25)

        if carousel == self.carouselView {
            createCarouselSubview(list: myDrawingsList, thumbnailView: thumbnailView, nameLabel: nameLabel, protectionButton: protectionButton, visibilityLabel: visibilityLabel, cardsView: cardsView, index: index, protectionButtonStatus: true)
        }

        if carousel == self.carousel2View {
            createCarouselSubview(list: publicDrawingsList, thumbnailView: thumbnailView, nameLabel: nameLabel, protectionButton: protectionButton, visibilityLabel: visibilityLabel, cardsView: cardsView, index: index, protectionButtonStatus: false)
        }
        return cardsView
    }

    func createCarouselSubview(list: [ExtendedDrawingModel], thumbnailView: UIImageView, nameLabel: UILabel, protectionButton: UIButton, visibilityLabel: UILabel, cardsView: UIView, index: Int, protectionButtonStatus: Bool ) {
        var thumbnail = UIImage()

        if list[index].thumbnail != "" {
            if let decodedData = Data(base64Encoded: list[index].thumbnail, options: .ignoreUnknownCharacters) {
                thumbnail = UIImage(data: decodedData)!
            }
        }

        thumbnailView.image = thumbnail
        nameLabel.text = list[index].properties.name
        protectionButton.setTitle("🔒", for: .normal)
        protectionButton.tag = index
        protectionButton.addTarget(self, action: #selector(protectionToggle), for: .touchUpInside)
        if (list[index] as ExtendedDrawingModel).properties.protection.active {
            protectionButton.setTitle("🔒", for: .normal)
        } else {
            protectionButton.setTitle("🔓", for: .normal)
        }
        if (list[index] as ExtendedDrawingModel).properties.visibility == "public" {
            visibilityLabel.text = "🙉"
        } else {
            visibilityLabel.text = "🙈"
        }

        if !protectionButtonStatus {
            protectionButton.isEnabled = false
        }
        cardsView.addSubview(thumbnailView)
        cardsView.addSubview(nameLabel)
        cardsView.addSubview(protectionButton)
        cardsView.addSubview(visibilityLabel)
    }

    func carousel(_ carousel: iCarousel, didSelectItemAt index: Int) {
        if carousel == self.carouselView {
            openMySelectedDrawing(index: index)
        }
        if carousel == self.carousel2View {
            openPublicSelectedDrawing(index: index)
        }
    }

    func carousel(_ carousel: iCarousel, valueFor option: iCarouselOption, withDefault value: CGFloat) -> CGFloat {
        if option == iCarouselOption.spacing {
            return value * 1.2
        }
        return value
    }

    override func awakeFromNib() {
        super.awakeFromNib()
    }

    func openMySelectedDrawing(index: Int) {
        if myDrawingsList[index].properties.mode == "stroke" {
            performSegue(withIdentifier: "JoinStrokeDrawingSegue", sender: nil)
        } else if myDrawingsList[index].properties.mode == "pixel" {
            performSegue(withIdentifier: "JoinPixelDrawingSegue", sender: nil)
        }
    }

    func openPublicSelectedDrawing(index: Int) {
        if !publicDrawingsList[index].properties.protection.active {
            if publicDrawingsList[index].properties.mode == "stroke" {
                performSegue(withIdentifier: "JoinStrokeDrawingSegue", sender: nil)
            } else if publicDrawingsList[index].properties.mode == "pixel" {
                performSegue(withIdentifier: "JoinPixelDrawingSegue", sender: nil)
            }
        } else {
            showAlert(index: index)
        }
    }

    @objc func protectionToggle(sender:UIButton) {
        let index = sender.tag
            toggleProtectionAlert(index: index)

    }

    func toggleProtectionAlert(index: Int) {
          var alert = UIAlertController()
        if !myDrawingsList[index].properties.protection.active {
            alert = UIAlertController(title: "Image non protégée",
                                      message: "Entrez le mot de passe que vous voulez à l'image",
                                          preferredStyle: .alert)
        } else if myDrawingsList[index].properties.protection.active {
            alert = UIAlertController(title: "Image protégée",
                                          message: "Entrez le mot de passe pour enlever la protection sur l'image",
                                          preferredStyle: .alert)
        }
        // Submit button
        let submitAction = UIAlertAction(title: "Soumettre", style: .default, handler: { _ -> Void in
            // Get 1st TextField's text
            let inputPassword = alert.textFields![0]
            print(inputPassword.text!)
            if self.validatePassword(inputPassword: inputPassword.text!) {
               //actions de validation complétée
                self.togglePadlock(index: index, password: inputPassword.text!)

            } else if !self.validatePassword(inputPassword: inputPassword.text!) {
                inputPassword.text! = ""
            }
        })
        // Cancel button
        let cancel = UIAlertAction(title: "Annuler", style: .destructive, handler: { _ -> Void in })
        alert.addTextField { (textField: UITextField) in
            textField.keyboardAppearance = .dark
            textField.keyboardType = .default
            textField.autocorrectionType = .default
            textField.placeholder = "Mot de passe"
            textField.clearButtonMode = .whileEditing
            textField.isSecureTextEntry = true
        }
        // Add action buttons and present the Alert
        alert.addAction(submitAction)
        alert.addAction(cancel)
        present(alert, animated: true, completion: nil)

    }

    func togglePadlock(index:Int, password: String) { //here is where we change the protection status of a drawing
        if myDrawingsList[index].properties.protection.active {
            //we remove the protection here
            carouselView.reloadData()
        } else {
            //we set the protection status here
            //we can use the parameter password to set the password to the drawing
            carouselView.reloadData()
        }
    }

    private func insertNewOwnerDrawing(drawing: ExtendedDrawingModel) {
        myDrawingsList.append(drawing)
        carouselView.reloadData()
    }

    private func insertNewPublicDrawing(drawing: ExtendedDrawingModel) {
        if !AccountManager.sharedInstance.isMyself(id: drawing.properties.owner.id) { //Avoid duplicates with owner list
            publicDrawingsList.append(drawing)
            carousel2View.reloadData()
        }
    }

    func validatePassword(inputPassword: String) -> Bool {
        // MARK: - insert logic for pwd validation here
        return true
    }

    func showAlert(index: Int) {
        let alert = UIAlertController(title: "Image protégée",
                                      message: "Entrez le mot de passe pour accéder à l'image",
                                      preferredStyle: .alert)
        // Submit button
        let submitAction = UIAlertAction(title: "Soumettre", style: .default, handler: { _ -> Void in
            // Get 1st TextField's text
            let inputPassword = alert.textFields![0]
            print(inputPassword.text!)
            if self.validatePassword(inputPassword: inputPassword.text!) {
                if (self.myDrawingsList[index] as ExtendedDrawingModel).properties.mode == DrawingTypes.Pixel {
                    self.performSegue(withIdentifier: "JoinPixelDrawingSegue", sender: self)
                } else if (self.myDrawingsList[index] as ExtendedDrawingModel).properties.mode == DrawingTypes.Stroke {
                    self.performSegue(withIdentifier: "JoinStrokeDrawingSegue", sender: self)
                }
            } else if !self.validatePassword(inputPassword: inputPassword.text!) {
                inputPassword.text! = ""
            }
        })
        // Cancel button
        let cancel = UIAlertAction(title: "Annuler", style: .destructive, handler: { _ -> Void in })
        alert.addTextField { (textField: UITextField) in
            textField.keyboardAppearance = .dark
            textField.keyboardType = .default
            textField.autocorrectionType = .default
            textField.placeholder = "Mot de passe"
            textField.clearButtonMode = .whileEditing
            textField.isSecureTextEntry = true
        }
        // Add action buttons and present the Alert
        alert.addAction(submitAction)
        alert.addAction(cancel)
        present(alert, animated: true, completion: nil)
    }

    private func loadDrawingThumbnail(drawing: OnlineDrawingModel) -> Promise<String> {
        return RestManager.getDrawingThumbnail(drawingId: drawing.id)
            .then { response -> String in
                return (response.success) ? response.data! : ""
        }
    }

    private func loadOwnerDrawing(drawing: OnlineDrawingModel) {
        var extendedDrawing = ExtendedDrawingModel(drawing: drawing)
        self.loadDrawingThumbnail(drawing: drawing).then { response -> Void in
            extendedDrawing.thumbnail = response
            self.insertNewOwnerDrawing(drawing: extendedDrawing)
        }
    }

    private func loadPublicDrawing(drawing: OnlineDrawingModel) {
        var extendedDrawing = ExtendedDrawingModel(drawing: drawing)
        self.loadDrawingThumbnail(drawing: drawing).then { response -> Void in
            extendedDrawing.thumbnail = response
            self.insertNewPublicDrawing(drawing: extendedDrawing)
        }
    }

    private func loadOnlineDrawings() {
        self.loadOnlineDrawings(callback: self.loadOwnerDrawing,
                                userId: AccountManager.sharedInstance.user?.id)
        self.loadOnlineDrawings(callback: self.loadPublicDrawing, visibility: "public")
    }

    private func loadOnlineDrawings(callback: @escaping (OnlineDrawingModel) -> Void,
                                    userId: String? = nil, visibility: String? = nil) {
        RestManager.getDrawingsListPage(page: 1, userId: userId, visibility: visibility)
            .then { response -> Void in
                if response.success {
                    for drawing in (response.data?.docs)! {
                        callback(drawing)
                    }
                    if (response.data?.pages)! > 1 {
                        //Load all other pages (other than page 1)
                        self.loadOnlineDrawingsPages(from: 2, to: (response.data?.pages)!,
                                                     callback: callback, userId: userId,
                                                     visibility: visibility)
                    }
                } else {
                    print("Failed to get drawings page: 1")
                }
            }.catch { error in
                print("Unexpected error during get drawings: \(error). At page: 1")
        }
    }

    // swiftlint:disable identifier_name
    private func loadOnlineDrawingsPages(from: Int, to: Int,
                                         callback: @escaping (OnlineDrawingModel) -> Void,
                                         userId: String? = nil, visibility: String? = nil) {
        for index in from...to {
            RestManager.getDrawingsListPage(page: index, userId: userId, visibility: visibility)
                .then { response -> Void in
                    if response.success {
                        for drawing in (response.data?.docs)! {
                            callback(drawing)
                        }
                    } else {
                        print("Failed to get drawings page: \(index)")
                    }
                }.catch { error in
                    print("Unexpected error during get drawings: \(error). At page: \(index)")
            }
        }
    }
    // swiftlint:enable identifier_name

    internal struct ExtendedDrawingModel {
        var properties: OnlineDrawingModel
        var thumbnail: String = ""

        init(drawing: OnlineDrawingModel) {
            self.properties = drawing
        }
    }
}
