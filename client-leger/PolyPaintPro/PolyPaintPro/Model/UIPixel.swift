//
//  UIPixel.swift
//  PolyPaintPro
//
//  Created by Kenny Nguyen on 2018-04-09.
//  Copyright © 2018 Les Pods c'est pour les lunchs. All rights reserved.
//

import Foundation
import UIKit

struct UIPixel: Codable {
    let point: CGPoint
    let color: SKStrokeColor
    init(point: CGPoint, color: SKStrokeColor) {
        self.point = point
        self.color = color
    }
}
