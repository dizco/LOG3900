//
//  TextFieldValidator.swift
//  PolyPaintPro
//
//  Created by Gabriel Bourgault on 2018-03-20.
//  Copyright © 2018 Les Pods c'est pour les lunchs. All rights reserved.
//

import Foundation
import SpriteKit

class TextFieldValidator: NSObject, UITextFieldDelegate {
    private var minValue: Int
    private var maxValue: Int

    init(minValue: Int, maxValue: Int) {
        self.minValue = minValue
        self.maxValue = maxValue
    }

    func textField(_ textField: UITextField, shouldChangeCharactersIn range: NSRange,
                   replacementString string: String) -> Bool {
        let allowedCharacters = CharacterSet.decimalDigits
        let characterSet = CharacterSet(charactersIn: string)

        if !allowedCharacters.isSuperset(of: characterSet) {
            return false
        }

        let inputNumber = self.getIntValueForTextField(textField: textField, addedString: string)
        if inputNumber < self.minValue {
            textField.text! = String(self.minValue)
            return false
        }
        if inputNumber > self.maxValue {
            textField.text! = String(self.maxValue)
            return false
        }

        return true
    }

    private func getIntValueForTextField(textField: UITextField, addedString: String) -> Int {
        var inputString = ""
        if textField.text != nil {
            inputString += textField.text!
        }
        inputString += addedString
        return Int(inputString)!
    }
}
