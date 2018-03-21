//
//  SKStroke.swift
//  PolyPaintPro
//
//  Created by Kenny Nguyen on 2018-03-20.
//  Copyright © 2018 Les Pods c'est pour les lunchs. All rights reserved.
//

import Foundation
import SpriteKit

class SKStroke: SKShapeNode {
    var id: String = UUID().uuidString

    func setReceivedUuid(uuid: String) {
        self.id = uuid
    }
    // TO-DO : Use author name to manage the stack.
}
