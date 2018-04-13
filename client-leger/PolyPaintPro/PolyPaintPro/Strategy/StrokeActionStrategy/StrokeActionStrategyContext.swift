//
//  StrokeActionStrategyContext.swift
//  PolyPaintPro
//
//  Created by Kenny Nguyen on 2018-04-03.
//  Copyright © 2018 Les Pods c'est pour les lunchs. All rights reserved.
//

import Foundation

final class StrokeActionStrategyContext {
    init(scene: StrokeEditorScene, incomingAction: IncomingActionMessage) {
        let actionName = incomingAction.action.name

        switch actionName {
        case StrokeActionNameConstants.addActionName.rawValue:
            AddStrokeActionStrategy().applyReceived(scene: scene, incomingAction: incomingAction)

        case StrokeActionNameConstants.resetActionName.rawValue:
            ResetStrokeActionStrategy().applyReceived(scene: scene, incomingAction: incomingAction)

        case StrokeActionNameConstants.lockActionName.rawValue:
            LockStrokeActionStrategy().applyReceived(scene: scene, incomingAction: incomingAction)

        case StrokeActionNameConstants.unlockActionName.rawValue:
            UnlockStrokeActionStrategy().applyReceived(scene: scene, incomingAction: incomingAction)

        case StrokeActionNameConstants.transformActionName.rawValue:
            TransformStrokeActionStrategy().applyReceived(scene: scene, incomingAction: incomingAction)

        case StrokeActionNameConstants.replaceActionName.rawValue:
            ReplaceStrokeActionStrategy().applyReceived(scene: scene, incomingAction: incomingAction)

        default:
            print("Unknown IncomingStrokeAction.")
        }
    }
}
