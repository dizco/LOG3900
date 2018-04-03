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

class StrokeEditorViewController: EditorViewController, ActionSocketManagerDelegate, DrawingToolsViewDelegate, StrokeToolsViewDelegate {
    // MARK: - Scene
    private var scene = StrokeEditorScene()

    override func didReceiveMemoryWarning() {
        super.didReceiveMemoryWarning()
    }

    override func viewDidLoad() {
        super.viewDidLoad()

        // StrokeEditor Embassy
        SocketManager.sharedInstance.actionDelegate = self
        drawingSettingsView.delegate = self
        toolsView.strokeDelegate = self

        self.scene = StrokeEditorScene(size: view.frame.size)
        let skView = view as? SKView
        self.scene.scaleMode = .fill
        skView!.presentScene(self.scene)
    }

    override func viewDidAppear(_ animated: Bool) {
        super.viewDidAppear(animated)
    }

    // MARK: - ActionSocketManagerDelegate
    func managerDidReceiveAction(data: Data) {
        do {
            print("Action data received.")
            let decoder = JSONDecoder()
            let incomingAction = try decoder.decode(IncomingActionMessage.self, from: data)
            self.scene.applyReceived(incomingAction: incomingAction)
        } catch let error {
            print(error)
        }
    }

    // MARK: - DrawingToolsViewDelegate
    func updateColorValues(red: Int, green: Int, blue: Int, alpha: Int) {
        self.scene.updateColorValues(red: red, green: green, blue: blue, alpha: alpha)
    }

    func updateBrushSize(size: Int) {
        self.scene.updateBrushSize(size: size)
    }

    // MARK: - ToolsViewDelegate
    func updateEditingMode(mode: StrokeEditingMode) {
        self.scene.setEditingMode(mode: mode)
    }

    func resetCanvas() {
        self.scene.resetCanvas()

        // Special case: Preventing the reset infinite loop.
        self.scene.sendEditorAction(actionId: StrokeActionIdConstants.reset.rawValue)
    }

    func stack() {
        self.scene.stack()
    }

    func unstack() {
        self.scene.unstack()
    }

}
