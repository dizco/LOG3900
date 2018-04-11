//
//  LockStrokeActionStrategy.swift
//  PolyPaintPro
//
//  Created by Kenny Nguyen on 2018-04-11.
//  Copyright © 2018 Les Pods c'est pour les lunchs. All rights reserved.
//

import Foundation

final class LockStrokeActionStrategy: StrokeActionStrategy {
    func applyReceived(scene: StrokeEditorScene, incomingAction: IncomingActionMessage) {
        for strokeUuid in incomingAction.delta.remove {
            self.lockStrokeWith(scene: scene, strokeUuid: strokeUuid)
        }
    }

    private func lockStrokeWith(scene: StrokeEditorScene, strokeUuid: String) {
        // We're looking for the right stroke to change to lock...

        // ... in our local strokes list ...
        scene.enumerateChildNodes(withName: scene.COMPLETESTROKE, using: {node, stop in
            let currentNode = node as! SKStroke
            if currentNode.id == strokeUuid {
                currentNode.lock()
            }
        })

        // ... and in our online strokes list.
        scene.enumerateChildNodes(withName: scene.RECEIVEDSTROKE, using: {node, stop in
            let currentNode = node as! SKStroke
            if currentNode.id == strokeUuid {
                currentNode.lock()
            }
        })
    }
}
