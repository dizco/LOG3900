//
//  BuildStrokeActionStrategyContext.swift
//  PolyPaintPro
//
//  Created by Kenny Nguyen on 2018-04-03.
//  Copyright © 2018 Les Pods c'est pour les lunchs. All rights reserved.
//

import Foundation

final class BuildStrokeActionStrategyContext {
    let outgoingActionMessage: OutgoingActionMessage?

    init(scene: StrokeEditorScene, actionId: Int, strokeUuid: String, stroke: SKStroke? = nil) {
        switch actionId {
        case StrokeActionIdConstants.add.rawValue:
            self.outgoingActionMessage = AddBuildStrokeActionStrategy().buildOutgoingAction(scene: scene, actionId: actionId, strokeUuid: strokeUuid, stroke: stroke)

        case StrokeActionIdConstants.reset.rawValue:
            self.outgoingActionMessage = ResetBuildStrokeActionStrategy().buildOutgoingAction(scene: scene, actionId: actionId, strokeUuid: strokeUuid)

        case StrokeActionIdConstants.replace.rawValue:
            self.outgoingActionMessage = ReplaceBuildStrokeActionStrategy().buildOutgoingAction(scene: scene, actionId: actionId, strokeUuid: strokeUuid)

        default:
            self.outgoingActionMessage = nil
        }
    }

    func getOutgoingMessage() -> OutgoingActionMessage? {
        return self.outgoingActionMessage
    }
}
