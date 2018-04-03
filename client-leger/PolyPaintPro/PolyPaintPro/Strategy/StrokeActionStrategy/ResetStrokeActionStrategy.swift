//
//  ResetStrokeActionStrategy.swift
//  PolyPaintPro
//
//  Created by Kenny Nguyen on 2018-04-03.
//  Copyright © 2018 Les Pods c'est pour les lunchs. All rights reserved.
//

import Foundation

final class ResetStrokeActionStrategy: StrokeActionStrategy {
    func applyReceived(scene: StrokeEditorScene, incomingAction: IncomingActionMessage) {
        scene.resetCanvas()
    }
}
