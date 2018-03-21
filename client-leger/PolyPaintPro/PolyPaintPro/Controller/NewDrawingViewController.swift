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

    @IBOutlet weak var drawingNameTextField: UITextField!
    @IBOutlet weak var drawingTypePickerview: UIPickerView!

    override func viewDidLoad() {
        super.viewDidLoad()
        drawingTypePickerview.delegate = self
        drawingTypePickerview.dataSource = self
    }

    override func didReceiveMemoryWarning() {
        super.didReceiveMemoryWarning()
    }

    override func prepare(for segue: UIStoryboardSegue, sender: Any?) {
        let vc = segue.destination as! EditorViewController
        vc.connectionStatus = connectionStatus
    }

    @IBAction func createDrawingButton(_ sender: UIButton) {
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
