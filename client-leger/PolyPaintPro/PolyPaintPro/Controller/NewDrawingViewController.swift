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
    var connectionStatus = true
    var selectedDrawingType: String = "" //this variable represents the type of drawing the user selected


    @IBOutlet weak var drawingNameTextField: UITextField!
    @IBOutlet weak var drawingTypePickerview: UIPickerView!

    @IBAction func createDrawingButton(_ sender: UIButton) {
    }

    //these 4 functions are required for the uipicker to work properly
    func numberOfComponents(in pickerView: UIPickerView) -> Int {
        return 1
    }

    func pickerView(_ pickerView: UIPickerView, numberOfRowsInComponent component: Int) -> Int {
        return Drawing.Types.count
    }

    func pickerView(_ pickerView: UIPickerView, titleForRow row: Int, forComponent component: Int) -> String? {
        return Drawing.Types[row]
    }
    func pickerView(_ pickerView: UIPickerView, didSelectRow row: Int, inComponent component: Int) {
        //enter here the code for the selected type
        selectedDrawingType = Drawing.Types[row]
    }

    override func viewDidLoad() {
        super.viewDidLoad()
        drawingTypePickerview.delegate = self
        drawingTypePickerview.dataSource = self
    }

    override func didReceiveMemoryWarning() {
        super.didReceiveMemoryWarning()
    }

    override func prepare(for segue: UIStoryboardSegue, sender: Any?) {
        let vc = segue.destination as! ViewController
        vc.connectionStatus = connectionStatus
    }
}
