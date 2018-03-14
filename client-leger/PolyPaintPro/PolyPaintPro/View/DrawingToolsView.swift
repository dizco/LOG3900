//
//  DrawingToolsView.swift
//  PolyPaintPro
//
//  Created by Fred Habsfan on 2018-02-20.
//  Copyright © 2018 Les Pods c'est pour les lunchs. All rights reserved.
//

import Foundation
import UIKit

class DrawingToolsView: UIView {
    var redValue: Int = 0
    var greenValue: Int = 0
    var  blueValue: Int = 0
    var alphaValue: Int = 100
    var widthValue: Int = 11

    @IBOutlet weak var colorPreview: UIView!
    @IBOutlet weak var redField: UITextField!
    @IBOutlet weak var greenField: UITextField!
    @IBOutlet weak var blueField: UITextField!
    @IBOutlet weak var alphaField: UITextField!
    @IBOutlet weak var sizeField: UITextField!
    @IBOutlet weak var redSlider: UISlider!
    @IBOutlet weak var greenSlider: UISlider!
    @IBOutlet weak var blueSlider: UISlider!
    @IBOutlet weak var alphaSlider: UISlider!
    @IBOutlet weak var sizeSlider: UISlider!
    
    @IBAction func redSliderChanged(_ sender: UISlider) {
        redValue = lroundf(sender.value)
        redField.text! = "\(redValue)"
        updateColor()
    }
    @IBAction func greenSliderChanged(_ sender: UISlider) {
        greenValue = lroundf(sender.value)
        greenField.text! = "\(greenValue)"
        updateColor()
    }
    @IBAction func blueSliderChanged(_ sender: UISlider) {
        blueValue = lroundf(sender.value)
        blueField.text! = "\(blueValue)"
        updateColor()
    }
    @IBAction func alphaSliderChanged(_ sender: UISlider) {
        alphaValue = lroundf(sender.value)
        alphaField.text! = "\(alphaValue)"
        updateColor()
    }
    @IBAction func sizeSliderChanged(_ sender: UISlider) {
        widthValue = lroundf(sender.value)
        sizeField.text! = "\(widthValue)"
        updateSize()
    }
    @IBAction func redTextFieldChanged(_ sender: UITextField) {
        redValue = (redField.text as! NSString).integerValue
        redSlider.value = Float(redValue)
        updateColor()
    }
    @IBAction func greenTextFieldChanged(_ sender: UITextField) {
        greenValue = (greenField.text as! NSString).integerValue
        greenSlider.value = Float(greenValue)
        updateColor()
    }
    @IBAction func blueTextFieldChanged(_ sender: Any) {
        blueValue = (blueField.text as! NSString).integerValue
        blueSlider.value = Float(blueValue)
        updateColor()
    }
    @IBAction func alphaTextFieldChanged(_ sender: UITextField) {
        alphaValue = (alphaField.text as! NSString).integerValue
        alphaSlider.value = Float(alphaValue)
        updateColor()
    }
    @IBAction func sizeTextFieldChanged(_ sender: UITextField) {
        widthValue = (sizeField.text as! NSString).integerValue
        sizeSlider.value = Float(widthValue)
        updateSize()

    }

    func updateColor() {
        colorPreview.backgroundColor = UIColor(red: CGFloat(redValue)/255, green: CGFloat(greenValue)/255, blue: CGFloat(blueValue)/255, alpha:CGFloat(alphaValue)/100)
        PixelEditorViewController().red = CGFloat(redValue)
        PixelEditorViewController().green = CGFloat(greenValue)
        PixelEditorViewController().blue = CGFloat(blueValue)
    }

    func updateSize() {
        PixelEditorViewController().brushWidth = CGFloat(widthValue)
    }

    override init(frame: CGRect) {
        super.init(frame: frame)
        redField.text! = "\(redValue)"
        redSlider.value = Float(redValue)
        greenField.text! = "\(greenValue)"
        greenSlider.value = Float(greenValue)
        blueField.text! = "\(blueValue)"
        blueSlider.value = Float(blueValue)
        alphaField.text! = "\(alphaValue)"
        alphaSlider.value = Float(alphaValue)
    }

    required init?(coder aDecoder: NSCoder) {
        super.init(coder: aDecoder)
    }
}
