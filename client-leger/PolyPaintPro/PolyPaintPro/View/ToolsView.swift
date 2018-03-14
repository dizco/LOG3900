//
//  ToolsView.swift
//  PolyPaintPro
//
//  Created by Fred Habsfan on 2018-03-11.
//  Copyright © 2018 Les Pods c'est pour les lunchs. All rights reserved.
//

import UIKit

class ToolsView: UIView {
    @IBOutlet weak var penButton: UIButton!
    @IBOutlet weak var eraseButton: UIButton!
    @IBOutlet weak var cutButton: UIButton!
    @IBOutlet weak var pasteButton: UIButton!
    @IBOutlet weak var resetButton: UIButton!
    @IBOutlet weak var stackButton: UIButton!
    @IBOutlet weak var unstackButton: UIButton!
    @IBOutlet weak var settingsButton: UIButton!

    override init(frame: CGRect) {
        super.init(frame: frame)
    }

    required init?(coder aDecoder: NSCoder) {
        super.init(coder: aDecoder)
    }
    
    //action called for the pen
    @IBAction func penButton(_ sender: UIButton) {
        resetButtons(sender: sender, filename: "pencil")
    }
    //action called for the eraser
    @IBAction func eraseButton(_ sender: UIButton) {
        resetButtons(sender: sender, filename: "eraser")
    }
    //action called for the cut action
    @IBAction func cutButton(_ sender: UIButton) {
       resetButtons(sender: sender, filename: "cut")
    }
    //action called for the paste action
    @IBAction func pasteButton(_ sender: UIButton) {
        resetButtons(sender: sender, filename: "duplicate")
    }
    //action called for the reset action
    @IBAction func resetButton(_ sender: UIButton) {
        resetButtons(sender: sender, filename: "reset")
    }
    //action called for the stack action
    @IBAction func stackButton(_ sender: UIButton) {
        resetButtons(sender: sender, filename: "")
        sender.setTitleColor(.white, for: .normal)
    }
    //action called for the unstack action
    @IBAction func unstackButton(_ sender: UIButton) {
        resetButtons(sender: sender, filename: "")
       sender.setTitleColor(.white, for: .normal)
    }
    //action called for the advanced settings
    @IBAction func settingsButton(_ sender: UIButton) {
    }

    func setDefault() {
        let origImage = UIImage(named: "pencil")
        let tintedImage = origImage?.withRenderingMode(.alwaysTemplate)
        penButton.setImage(tintedImage, for: .normal)
        penButton.tintColor = .white
    }

    func resetButtons(sender: UIButton, filename: String) {
        resetUnicode()
        let penOrigImage = UIImage(named: "pencil")
        let penTintedImage = penOrigImage?.withRenderingMode(.alwaysTemplate)
        penButton.setImage(penTintedImage, for: .normal)
        penButton.tintColor = .black

        let eraserOrigImage = UIImage(named: "eraser")
        let eraserTintedImage = eraserOrigImage?.withRenderingMode(.alwaysTemplate)
        eraseButton.setImage(eraserTintedImage, for: .normal)
        eraseButton.tintColor = .black

        let cutOrigImage = UIImage(named: "cut")
        let cutTintedImage = cutOrigImage?.withRenderingMode(.alwaysTemplate)
        cutButton.setImage(cutTintedImage, for: .normal)
        cutButton.tintColor = .black

        let pasteOrigImage = UIImage(named: "duplicate")
        let pasteTintedImage = pasteOrigImage?.withRenderingMode(.alwaysTemplate)
        pasteButton.setImage(pasteTintedImage, for: .normal)
        pasteButton.tintColor = .black

        let resetOrigImage = UIImage(named: "reset")
        let resetTintedImage = resetOrigImage?.withRenderingMode(.alwaysTemplate)
        resetButton.setImage(resetTintedImage, for: .normal)
        resetButton.tintColor = .black

        let origImage = UIImage(named: filename)
        let tintedImage = origImage?.withRenderingMode(.alwaysTemplate)
        sender.setImage(tintedImage, for: .normal)
        sender.tintColor = .white

    }

    private func resetUnicode() {
        stackButton.setTitleColor(.black, for: .normal)
        unstackButton.setTitleColor(.black, for: .normal)
    }
}
