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
    internal var connectionStatus = true
    internal var selectedDrawingType: String = DrawingTypes.Types[0] //type of drawing the user selected
    internal var visibility = true //visibility var for the drawing to be created
    internal var protection = false //protection variable for the drawing to be created

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
        passwordProtectionTextField.isUserInteractionEnabled = protection
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
    }

    @IBAction func createDrawingButton(_ sender: UIButton) {
        if (protection && passwordProtectionTextField.text!.count >= 5 && !drawingNameTextField.text!.isEmpty) || (!protection && !drawingNameTextField.text!.isEmpty) {
            if selectedDrawingType == DrawingTypes.Types[0] {
                performSegue(withIdentifier: "StrokeEditorSegue", sender: self)
                // TO-DO : Correctly send a drawing subscription the server
                /*
                if SocketManager.sharedInstance.getConnectionStatus() {
                    do {
                        let outgoingMessage = DrawingSubscription()
                        let encodedData = try JSONEncoder().encode(outgoingMessage)
                        SocketManager.sharedInstance.send(data: encodedData)
                    } catch let error {
                        print(error)
                    }
                }*/
            } else if selectedDrawingType == DrawingTypes.Types[1] {
                performSegue(withIdentifier: "PixelEditorSegue", sender: self)
            }
        } else {
            let alert = UIAlertController(title: "Attention", message: "Vous devez nommer votre dessin et définir un mot de passe de 5 caractères s'il est protégé", preferredStyle: .alert)

            alert.addAction(UIAlertAction(title: "ok", style: .default, handler: nil))

            self.present(alert, animated: true)
        }
    }

    @IBAction func visibilityChanged(_ sender: Any) {
        self.visibility = visibilitySegmentedControl.selectedSegmentIndex == 0 //toggles the visibility mode
    }

    @IBAction func protectionChanged(_ sender: UISwitch) {
        protection = !protection
        passwordProtectionTextField.isUserInteractionEnabled = protection
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
