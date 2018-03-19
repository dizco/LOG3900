//
//  StrokeEditorViewController.swift
//  PolyPaintPro
//
//  Created by Kenny Nguyen on 2018-03-09.
//  Copyright © 2018 Les Pods c'est pour les lunchs. All rights reserved.
//

import Foundation
import UIKit
import SpriteKit

class StrokeEditorViewController: EditorViewController, DrawingToolsViewDelegate, ToolsViewDelegate {
    // MARK: - Scene
    var scene = StrokeEditorScene()

    override func didReceiveMemoryWarning() {
        super.didReceiveMemoryWarning()
    }

    override func viewDidLoad() {
        super.viewDidLoad()
        drawingSettingsView.delegate = self
        toolsView.delegate = self

        self.scene = StrokeEditorScene(size: view.frame.size)
        let skView = view as? SKView
        self.scene.scaleMode = .fill
        skView!.presentScene(self.scene)
    }

    override func viewDidAppear(_ animated: Bool) {
        super.viewDidAppear(animated)
    }

    // MARK: - DrawingToolsViewDelegate
    func updateColorValues(red: Int, green: Int, blue: Int, opacity: Int) {
        self.scene.updateColorValues(red: red, green: green, blue: blue, opacity: opacity)
    }

    // MARK: - ToolsViewDelegate
    func updateEditingMode(mode: EditingMode) {
        self.scene.setEditingMode(mode: mode)
    }

    func resetCanvas() {
        self.scene.resetCanvas()
    }

    func stack() {
        self.scene.stack()
    }

    func unstack() {
        self.scene.unstack()
    }

}
