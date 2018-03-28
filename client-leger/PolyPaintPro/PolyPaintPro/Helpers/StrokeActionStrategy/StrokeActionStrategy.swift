//
//  StrokeActionStrategy.swift
//  PolyPaintPro
//
//  Created by Kenny Nguyen on 2018-03-28.
//  Copyright © 2018 Les Pods c'est pour les lunchs. All rights reserved.
//

import Foundation

protocol StrokeActionStrategy {
    func applyReceived(scene: StrokeEditorScene, incomingAction: IncomingActionMessage)
}
