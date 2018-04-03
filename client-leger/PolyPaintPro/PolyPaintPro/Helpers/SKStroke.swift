//
//  SKStroke.swift
//  PolyPaintPro
//
//  Created by Kenny Nguyen on 2018-03-20.
//  Copyright © 2018 Les Pods c'est pour les lunchs. All rights reserved.
//

import Foundation
import SpriteKit

struct SKStrokeColor: Codable {
    let red: CGFloat
    let green: CGFloat
    let blue: CGFloat
    let alphaValue: CGFloat

    init(red: CGFloat, green: CGFloat, blue: CGFloat, alpha: CGFloat) {
        self.red = red
        self.green = green
        self.blue = blue
        self.alphaValue = alpha
    }
}

struct SKStrokeDots: Codable {
    let wayPoints: [CGPoint]
    let start: CGPoint
    let end: CGPoint

    init(wayPoints: [CGPoint], start: CGPoint, end: CGPoint) {
        self.wayPoints = wayPoints
        self.start = start
        self.end = end
    }
}

class SKStroke: SKShapeNode {
    var wayPoints: [CGPoint] = []
    var start: CGPoint = CGPoint.zero
    var end: CGPoint = CGPoint.zero

    var red: CGFloat = 0.0
    var green: CGFloat = 0.0
    var blue: CGFloat = 0.0
    var alphaValue: CGFloat = 1.0

    var id: String = UUID().uuidString

    func saveParameters(color: SKStrokeColor, dots: SKStrokeDots) {
        self.red = color.red
        self.green = color.green
        self.blue = color.blue
        self.alphaValue = color.alphaValue

        self.wayPoints = dots.wayPoints
        self.start = dots.start
        self.end = dots.end
    }

    func setReceivedUuid(uuid: String) {
        self.id = uuid
    }
    // TO-DO : Use author name to manage the stack.
}
