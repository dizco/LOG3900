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

        if actionName == IncomingMessageConstants.addActionName.rawValue {
            AddStrokeActionStrategy().applyReceived(scene: scene, incomingAction: incomingAction)
        } else if actionName == IncomingMessageConstants.resetActionName.rawValue {
            ResetStrokeActionStrategy().applyReceived(scene: scene, incomingAction: incomingAction)
        }
    }
}
