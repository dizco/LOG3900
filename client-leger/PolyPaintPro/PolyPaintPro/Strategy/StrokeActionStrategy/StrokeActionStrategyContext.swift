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

        if actionName == StrokeActionNameConstants.addActionName.rawValue {
            AddStrokeActionStrategy().applyReceived(scene: scene, incomingAction: incomingAction)
        } else if actionName == StrokeActionNameConstants.resetActionName.rawValue {
            ResetStrokeActionStrategy().applyReceived(scene: scene, incomingAction: incomingAction)
        } else if actionName == StrokeActionNameConstants.replaceActionName.rawValue {
            ReplaceStrokeActionStrategy().applyReceived(scene: scene, incomingAction: incomingAction)
        }
    }
}
