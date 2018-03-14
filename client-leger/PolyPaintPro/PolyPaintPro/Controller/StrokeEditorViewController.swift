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

class StrokeEditorViewController: EditorViewController, UIGestureRecognizerDelegate {

    override func didReceiveMemoryWarning() {
        super.didReceiveMemoryWarning()
    }

    override func viewDidLoad() {
        super.viewDidLoad()
        let scene = StrokeEditorScene(size: view.frame.size)
        let skView = view as? SKView
        scene.scaleMode = .fill
        skView!.presentScene(scene)
    }

    override func viewDidAppear(_ animated: Bool) {
        super.viewDidAppear(animated)
    }

}
