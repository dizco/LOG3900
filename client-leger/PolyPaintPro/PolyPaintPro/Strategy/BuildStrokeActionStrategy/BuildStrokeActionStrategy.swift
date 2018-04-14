//
//  BuildStrokeActionStrategy.swift
//  PolyPaintPro
//
//  Created by Kenny Nguyen on 2018-04-03.
//  Copyright © 2018 Les Pods c'est pour les lunchs. All rights reserved.
//

import Foundation

protocol BuildStrokeActionStrategy {
    func buildOutgoingAction(scene: StrokeEditorScene, actionId: Int, drawingId: String, strokeUuid: String, stroke: SKStroke?) -> OutgoingActionMessage
}
