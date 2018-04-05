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
    var myDrawingsList: [OnlineDrawingModel] = []
    var publicDrawingsList: [OnlineDrawingModel] = []

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
        // Dispose of any resources that can be recreated.
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
        let thumbnailView = UIImageView(frame: CGRect(x: 0, y: 0, width: 300, height: 225))
        let nameLabel = UILabel(frame: CGRect(x: 0, y: 225, width: 225, height: 25))
        let protectionButton = UIButton(frame: CGRect(x: 225, y: 225, width: 37, height: 25))
        //let protectionLabel = UILabel(frame: CGRect(x: 225, y: 225, width: 37, height: 25))
        let visibilityLabel = UILabel(frame: CGRect(x: 262, y: 225, width: 37, height: 25))

        if carousel == self.carouselView {
            createCarouselSubview(list: myDrawingsList, thumbnailView: thumbnailView, nameLabel: nameLabel, protectionButton: protectionButton, visibilityLabel: visibilityLabel, cardsView: cardsView, index: index)
        }

        if carousel == self.carousel2View {
            createCarouselSubview(list: publicDrawingsList, thumbnailView: thumbnailView, nameLabel: nameLabel, protectionButton: protectionButton, visibilityLabel: visibilityLabel, cardsView: cardsView, index: index)
        }
        return cardsView
    }

    func createCarouselSubview(list: [OnlineDrawingModel], thumbnailView: UIImageView, nameLabel: UILabel, protectionButton: UIButton, visibilityLabel: UILabel, cardsView: UIView, index: Int ) {
        thumbnailView.image = UIImage(named: "background")
        nameLabel.text = list[index].name
        protectionButton.setTitle("🔒", for: .normal)
        protectionButton.addTarget(self, action: "toggleProtection", for: .touchUpInside)
        if (list[index] as OnlineDrawingModel).protection.active {
            protectionButton.setTitle("🔒", for: .normal)
        } else {
            protectionButton.setTitle("🔓", for: .normal)
        }
        if (list[index] as OnlineDrawingModel).visibility == "public" {
            visibilityLabel.text = "🙉"
        } else {
            visibilityLabel.text = "🙈"
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
        //here is where we get the drawings names and we should also get the protection and visibility status
    }

    func openMySelectedDrawing(index: Int) {
        if myDrawingsList[index].mode == "stroke" {
            performSegue(withIdentifier: "JoinStrokeDrawingSegue", sender: nil)
        } else if myDrawingsList[index].mode == "pixel" {
            performSegue(withIdentifier: "JoinPixelDrawingSegue", sender: nil)
        }
    }

    func openPublicSelectedDrawing(index: Int) {
        if !publicDrawingsList[index].protection.active {
            if publicDrawingsList[index].mode == "stroke" {
                performSegue(withIdentifier: "JoinStrokeDrawingSegue", sender: nil)
            } else if publicDrawingsList[index].mode == "pixel" {
                performSegue(withIdentifier: "JoinPixelDrawingSegue", sender: nil)
            }
        } else {
            print("alerte")
            showAlert(index: index)
        }
    }

    func toggleProtectionMode(_ sender: UIButton) {
        print("toggle protection mode for" )
        //continuer ici avec les alertes de protection
    }

    func insertNewDrawing(drawing: OnlineDrawingModel) {
        myDrawingsList.append(drawing)
        carouselView.reloadData()
    }

    func insertNewPublicDrawing(drawing: OnlineDrawingModel) {
        publicDrawingsList.append(drawing)
        carousel2View.reloadData()
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
        let submitAction = UIAlertAction(title: "Submit", style: .default, handler: { _ -> Void in
            // Get 1st TextField's text
            let inputPassword = alert.textFields![0]
            print(inputPassword.text!)
            if self.validatePassword(inputPassword: inputPassword.text!) {
                if (self.myDrawingsList[index] as OnlineDrawingModel).mode == DrawingTypes.Pixel {
                    self.performSegue(withIdentifier: "JoinPixelDrawingSegue", sender: self)
                } else if (self.myDrawingsList[index] as OnlineDrawingModel).mode == DrawingTypes.Stroke {
                    self.performSegue(withIdentifier: "JoinStrokeDrawingSegue", sender: self)
                }
            } else if !self.validatePassword(inputPassword: inputPassword.text!) {
                inputPassword.text! = ""
            }
        })
        // Cancel button
        let cancel = UIAlertAction(title: "Cancel", style: .destructive, handler: { _ -> Void in })
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

    private func loadOnlineDrawings() {
        firstly {
            RestManager.getDrawingsListPage(page: 1)
        }.then { response -> Void in
            if response.success {
                for drawing in (response.data?.docs)! {
                    self.insertNewDrawing(drawing: drawing)
                    self.insertNewPublicDrawing(drawing: drawing)
                }
                if (response.data?.pages)! > 1 {
                    //Load all other pages (other than page 1)
                    self.loadOnlineDrawingsPages(from: 2, to: (response.data?.pages)!)
                }
            } else {
                print("Failed to get drawings page: 1")
            }
        }.catch { error in
            print("Unexpected error during get drawings: \(error). At page: 1")
        }
    }

    // swiftlint:disable identifier_name
    private func loadOnlineDrawingsPages(from: Int, to: Int) {
        for index in from...to {
            RestManager.getDrawingsListPage(page: index).then { response -> Void in
                if response.success {
                    for drawing in (response.data?.docs)! {
                        self.insertNewDrawing(drawing: drawing)
                        self.insertNewPublicDrawing(drawing: drawing)
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
}
