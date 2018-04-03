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

    init(scene: StrokeEditorScene, actionId: Int, strokeUuid: String) {
        if actionId == ActionIdConstants.add.rawValue {
            self.outgoingActionMessage = AddBuildStrokeActionStrategy().buildOutgoingAction(scene: scene, actionId: actionId, strokeUuid: strokeUuid)
        } else {
            self.outgoingActionMessage = nil
        }
    }

    func getOutgoingMessage() -> OutgoingActionMessage? {
        return self.outgoingActionMessage
    }
}
