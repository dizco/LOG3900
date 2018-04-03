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

        if incomingAction.type == IncomingMessageConstants.strokeAction.rawValue {
            AddStrokeActionStrategy().applyReceived(scene: scene, incomingAction: incomingAction)
        }
    }
}
