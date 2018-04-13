//
//  ToolsView.swift
//  PolyPaintPro
//
//  Created by Fred Habsfan on 2018-03-11.
//  Copyright © 2018 Les Pods c'est pour les lunchs. All rights reserved.
//

import UIKit

protocol StrokeToolsViewDelegate: class {
    func updateEditingMode(mode: StrokeEditingMode)
    func resetCanvas()
    func stack()
    func unstack()
}

protocol PixelToolsViewDelegate: class {
    func updateEditingMode(mode: PixelEditingMode)
    func toggleFiltersToolsView()

}

protocol EditorViewControllerDelegate: class {
    func drawingSettingsFn()
    func toolsToggleFn()
    func getToolsShowing() -> Bool
    func getDrawingSettingsShowing() -> Bool

}

class ToolsView: UIView {
    weak var strokeDelegate: StrokeToolsViewDelegate?
    weak var pixelDelegate: PixelToolsViewDelegate?
    weak var editorDelegate: EditorViewControllerDelegate?

    @IBOutlet weak var penButton: UIButton!
    @IBOutlet weak var strokeEraseButton: UIButton!
    @IBOutlet weak var segmentEraseButton: UIButton!
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

    @IBAction func penButton(_ sender: UIButton) {
        resetButtons(sender: sender, filename: "pencil")
        self.strokeDelegate?.updateEditingMode(mode: StrokeEditingMode.ink)
        self.pixelDelegate?.updateEditingMode(mode: PixelEditingMode.ink)
    }

    @IBAction func eraseButton(_ sender: UIButton) {
        resetButtons(sender: sender, filename: "eraser")
        self.strokeDelegate?.updateEditingMode(mode: StrokeEditingMode.eraseByStroke)
        self.pixelDelegate?.updateEditingMode(mode: PixelEditingMode.select)
    }

    @IBAction func byPointEraserButton(_ sender: UIButton) {
        //TO-DO : remplacer limage et le nom ici par les bonnes images
       resetButtons(sender: sender, filename: "eraser2")
        self.strokeDelegate?.updateEditingMode(mode: StrokeEditingMode.eraseByPoint)
        self.pixelDelegate?.updateEditingMode(mode: PixelEditingMode.eraseByPoint)
    }

    @IBAction func pasteButton(_ sender: UIButton) {
    }

    @IBAction func resetButton(_ sender: UIButton) {
       self.strokeDelegate?.resetCanvas()
    }

    @IBAction func stackButton(_ sender: UIButton) {
        self.strokeDelegate?.stack()
    }

    @IBAction func unstackButton(_ sender: UIButton) {
        self.strokeDelegate?.unstack()
    }

    @IBAction func filtersButton(_ sender: UIButton) {
        self.pixelDelegate?.updateEditingMode(mode: PixelEditingMode.filter)
        if (self.editorDelegate?.getDrawingSettingsShowing())! {
            self.editorDelegate?.drawingSettingsFn()
            self.pixelDelegate?.toggleFiltersToolsView()
        } else {
            self.pixelDelegate?.toggleFiltersToolsView()
        }

    }

    @IBAction func settingsButton(_ sender: UIButton) {
    }

    @IBAction func exitEditorButton(_ sender: Any) {
       //ibaction that is executed when the exit button is pressed in both editor view controllers
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
        strokeEraseButton.setImage(eraserTintedImage, for: .normal)
        strokeEraseButton.tintColor = .black

        let cutOrigImage = UIImage(named: "eraser2")
        let cutTintedImage = cutOrigImage?.withRenderingMode(.alwaysTemplate)
        segmentEraseButton.setImage(cutTintedImage, for: .normal)
        segmentEraseButton.tintColor = .black

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
