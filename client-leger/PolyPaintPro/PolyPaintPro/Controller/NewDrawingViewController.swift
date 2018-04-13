//
//  NewDrawingViewController.swift
//  PolyPaintPro
//
//  Created by Fred Habsfan on 2018-03-12.
//  Copyright © 2018 Les Pods c'est pour les lunchs. All rights reserved.
//

import Foundation
import UIKit

class NewDrawingViewController: UIViewController, UIPickerViewDataSource, UIPickerViewDelegate {
    private var drawing: IncomingDrawing?
    internal var connectionStatus = true
    internal var selectedDrawingType: String = DrawingTypes.Types[0]
    internal var isPublic = true
    internal var isProtected = false

    @IBOutlet var scrollView: UIScrollView!
    @IBOutlet weak var drawingNameTextField: UITextField!
    @IBOutlet weak var drawingTypePickerview: UIPickerView!
    @IBOutlet var visibilitySegmentedControl: UISegmentedControl!
    @IBOutlet var protectionToggle: UISwitch!
    @IBOutlet var passwordProtectionTextField: UITextField!

    override func viewDidLoad() {
        super.viewDidLoad()
        drawingTypePickerview.delegate = self
        drawingTypePickerview.dataSource = self
        self.hideKeyboard()
        self.observeKeyboardNotification()
        passwordProtectionTextField.isUserInteractionEnabled = isProtected
    }

    override func viewDidLayoutSubviews() {
        scrollView.isScrollEnabled = true
        scrollView.contentSize = CGSize (width: scrollView.contentSize.width, height: 900)
        scrollView.contentOffset.y = 900 - 740
    }

    override func didReceiveMemoryWarning() {
        super.didReceiveMemoryWarning()
    }

    override func prepare(for segue: UIStoryboardSegue, sender: Any?) {
        let vc = segue.destination as! EditorViewController
        vc.connectionStatus = connectionStatus
        vc.drawing = self.drawing!
    }

    @IBAction func createDrawingButton(_ sender: UIButton) {
        if (self.connectionStatus) {
            self.createOnlineDrawing()
        }
        else {
            self.createOfflineDrawing()
        }
    }

    private func createOnlineDrawing() {
        if (isProtected && passwordProtectionTextField.text!.count >= 5 && !drawingNameTextField.text!.isEmpty)
            || (!isProtected && !drawingNameTextField.text!.isEmpty) {
            RestManager.postDrawing(name: drawingNameTextField.text!,
                                    mode: (selectedDrawingType == DrawingTypes.Types[0])
                                        ? DrawingTypes.Stroke : DrawingTypes.Pixel,
                                    visibility: (isPublic) ? "public" : "private",
                                    protectionActive: isProtected)
                .then { response -> Void in
                    RestManager.getDrawing(id: (response.data?.id)!)
                        .then { getResponse -> Void in
                            if getResponse.success {
                                if let active = getResponse.data?.users.active,
                                    let limit = getResponse.data?.users.limit,
                                    active < limit {
                                    self.drawing = getResponse.data
                                    self.transition()
                                } else {
                                    self.showAlert(message: "Le dessin est plein et ne peut pas accueillir un éditeur supplémentaire")
                                }
                            } else {
                                self.showAlert(message: "Le dessin a été créé avec succès, mais il a été impossible de le récupérer")
                            }
                        }.catch { error in
                            print("Error during get drawing: \(error)")
                            self.showAlert(message: "Le dessin a été créé avec succès, mais il a été impossible de le récupérer")
                    }
                }.catch { error in
                    print("Error during post drawing: \(error)")
                    self.showAlert(message: "Une erreur est survenue lors de la création du dessin")
            }
        } else {
            self.showAlert(message: "Vous devez nommer votre dessin et définir un mot de passe de 5 caractères s'il est protégé")
        }
    }

    private func createOfflineDrawing() {
        if !drawingNameTextField.text!.isEmpty {
            self.drawing = IncomingDrawing(id: "",
                                           name: drawingNameTextField.text!,
                                           mode: (selectedDrawingType == DrawingTypes.Types[0])
                                            ? DrawingTypes.Stroke : DrawingTypes.Pixel,
                                           owner: IncomingOwner(id: "", username: ""),
                                           protection: IncomingProtection(active: false),
                                           visibility: "",
                                           users: IncomingUsers(active: 1, limit: 1),
                                           strokes: [], pixels: [])
            self.transition()
        } else {
            self.showAlert(message: "Vous devez nommer votre dessin")
        }
    }

    private func showAlert(message: String) {
        let alert = UIAlertController(title: "Attention",
                                      message: message,
                                      preferredStyle: .alert)
        alert.addAction(UIAlertAction(title: "ok", style: .default, handler: nil))
        self.present(alert, animated: true)
    }

    private func transition() {
        if self.selectedDrawingType == DrawingTypes.Types[0] {
            performSegue(withIdentifier: "StrokeEditorSegue", sender: self)
        } else if self.selectedDrawingType == DrawingTypes.Types[1] {
            performSegue(withIdentifier: "PixelEditorSegue", sender: self)
        }
    }

    @IBAction func visibilityChanged(_ sender: Any) {
        self.isPublic = visibilitySegmentedControl.selectedSegmentIndex == 0 //toggles the visibility mode
    }

    @IBAction func protectionChanged(_ sender: UISwitch) {
        isProtected = !isProtected
        passwordProtectionTextField.isUserInteractionEnabled = isProtected
    }

    // MARK: - UIPickerView
    func numberOfComponents(in pickerView: UIPickerView) -> Int {
        return 1
    }

    func pickerView(_ pickerView: UIPickerView, numberOfRowsInComponent component: Int) -> Int {
        return DrawingTypes.Types.count
    }

    func pickerView(_ pickerView: UIPickerView, titleForRow row: Int, forComponent component: Int) -> String? {
        return DrawingTypes.Types[row]
    }

    func pickerView(_ pickerView: UIPickerView, didSelectRow row: Int, inComponent component: Int) {
        //enter here the code for the selected type
        selectedDrawingType = DrawingTypes.Types[row]
    }
}
