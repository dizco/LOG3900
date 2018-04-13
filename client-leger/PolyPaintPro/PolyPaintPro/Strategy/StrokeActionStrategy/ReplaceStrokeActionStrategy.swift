//
//  ReplaceStrokeActionStrategy.swift
//  PolyPaintPro
//
//  Created by Kenny Nguyen on 2018-04-03.
//  Copyright © 2018 Les Pods c'est pour les lunchs. All rights reserved.
//

import Foundation

final class ReplaceStrokeActionStrategy: StrokeActionStrategy {
    func applyReceived(scene: StrokeEditorScene, incomingAction: IncomingActionMessage) {
        // EraseByStroke
        if incomingAction.delta.add.isEmpty {
            for strokeToBeRemoved in incomingAction.delta.remove {
                scene.eraseByStrokeWith(strokeUuid: strokeToBeRemoved)
            }
        } else { // EraseByPoint
            AddStrokeActionStrategy().applyReceived(scene: scene, incomingAction: incomingAction)
            for strokeToBeRemoved in incomingAction.delta.remove {
                scene.eraseByStrokeWith(strokeUuid: strokeToBeRemoved)
            }
        }
    }
}
