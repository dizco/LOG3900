//
//  ReplaceBuildStrokeActionStrategy.swift
//  PolyPaintPro
//
//  Created by Kenny Nguyen on 2018-04-03.
//  Copyright © 2018 Les Pods c'est pour les lunchs. All rights reserved.
//

import Foundation

final class ReplaceBuildStrokeActionStrategy: BuildStrokeActionStrategy {
    func buildOutgoingAction(scene: StrokeEditorScene, actionId: Int, drawingId: String, strokeUuid: String, stroke: SKStroke? = nil) -> OutgoingActionMessage {

        // TO-DO: Differentiate between EraseByStroke and EraseByPoint

        // 1. Create the add and remove arrays
        var remove: [String] = []
        remove.append(strokeUuid)

        // 2. Create the OutgoingDelta
        let delta: OutgoingDelta = OutgoingDelta(remove: remove)

        // 3. Create the OutgoingActionMessage
        return OutgoingActionMessage(actionId: actionId, actionName: StrokeActionNameConstants.replaceActionName.rawValue,
                                     drawingId: drawingId, delta: delta)
    }
}
