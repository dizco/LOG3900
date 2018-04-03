//
//  ResetBuildStrokeActionStrategy.swift
//  PolyPaintPro
//
//  Created by Kenny Nguyen on 2018-04-03.
//  Copyright © 2018 Les Pods c'est pour les lunchs. All rights reserved.
//

import Foundation

final class ResetBuildStrokeActionStrategy: BuildStrokeActionStrategy {
    func buildOutgoingAction(scene: StrokeEditorScene, actionId: Int, strokeUuid: String) -> OutgoingActionMessage {
        let delta: OutgoingDelta = OutgoingDelta()

        // Create the OutgoingActionMessage
        return OutgoingActionMessage(actionId: actionId, actionName: StrokeActionNameConstants.resetActionName.rawValue, delta: delta)
    }
}
