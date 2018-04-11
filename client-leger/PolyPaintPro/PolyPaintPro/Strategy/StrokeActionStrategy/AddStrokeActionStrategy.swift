//
//  AddStrokeActionStrategy.swift
//  PolyPaintPro
//
//  Created by Kenny Nguyen on 2018-03-28.
//  Copyright © 2018 Les Pods c'est pour les lunchs. All rights reserved.
//

import Foundation
import SpriteKit

final class AddStrokeActionStrategy: StrokeActionStrategy {
    func applyReceived(scene: StrokeEditorScene, incomingAction: IncomingActionMessage) {
        if !AccountManager.sharedInstance.isMyself(id: incomingAction.author.id) {
            for receivedStroke in incomingAction.delta.add {
                self.drawReceived(scene: scene, stroke: receivedStroke)
            }
        }
    }

    private func drawReceived(scene: StrokeEditorScene, stroke: IncomingAdd) {
        let path = self.createReceivedPathWith(scene: scene, dotsArray: stroke.dots)
        let color = self.convertHexToUIColor(hex: stroke.strokeAttributes.color)!

        let shapeNode = SKStroke()
        shapeNode.path = path
        shapeNode.name = scene.RECEIVEDSTROKE
        shapeNode.setReceivedUuid(uuid: stroke.strokeUuid)

        shapeNode.strokeColor = color
        shapeNode.lineWidth = CGFloat(stroke.strokeAttributes.width)
        // Can't do stuff with strokeAttributes.height
        shapeNode.lineJoin = CGLineJoin.round
        shapeNode.lineCap = CGLineCap.round

        shapeNode.generateDotsFromPath()
        scene.addChild(shapeNode)
    }

    private func convertHexToUIColor(hex: String) -> UIColor? {
        // https://cocoacasts.com/from-hex-to-uicolor-and-back-in-swift
        // thank you mr skeltal
        var rgb: UInt32 = 0

        var red: CGFloat = 0.0
        var green: CGFloat = 0.0
        var blue: CGFloat = 0.0
        var alpha: CGFloat = 0.0

        var hexSanitized = hex.trimmingCharacters(in: .whitespacesAndNewlines)
        hexSanitized = hexSanitized.replacingOccurrences(of: "#", with: "")
        let length = hexSanitized.count

        guard Scanner(string: hexSanitized).scanHexInt32(&rgb) else { return nil }

        if length == 6 {
            red = CGFloat((rgb & 0xFF0000) >> 16) / 255.0
            green = CGFloat((rgb & 0x00FF00) >> 8) / 255.0
            blue = CGFloat(rgb & 0x0000FF) / 255.0

        } else if length == 8 {
            alpha = CGFloat((rgb & 0xFF000000) >> 24) / 255.0
            red = CGFloat((rgb & 0x00FF0000) >> 16) / 255.0
            green = CGFloat((rgb & 0x0000FF00) >> 8) / 255.0
            blue = CGFloat(rgb & 0x000000FF) / 255.0
        } else {
            return nil
        }

        return UIColor(red: red, green: green, blue: blue, alpha: alpha)
    }

    private func createReceivedPathWith(scene: StrokeEditorScene, dotsArray: [IncomingDots]) -> CGMutablePath {
        let strokeStart: CGPoint
        let strokeEnd: CGPoint
        var dotsArrayToUse: [IncomingDots] = dotsArray
        var isOneOrTwoDots = false

        if dotsArray.count == 1 {
            isOneOrTwoDots = true
            strokeStart = self.convertDotToCGPoint(scene: scene, dot: dotsArray.first!)
            strokeEnd = strokeStart
        } else if dotsArray.count == 2 {
            isOneOrTwoDots = true
            strokeStart = self.convertDotToCGPoint(scene: scene, dot: dotsArray.first!)
            strokeEnd = self.convertDotToCGPoint(scene: scene, dot: dotsArray.last!)
        } else {
            strokeStart = self.convertDotToCGPoint(scene: scene, dot: dotsArray.first!)
            strokeEnd = self.convertDotToCGPoint(scene: scene, dot: dotsArray.last!)
            let tmpDotsArray = dotsArray.dropFirst()
            dotsArrayToUse = Array(tmpDotsArray.dropLast())
        }

        let path = CGMutablePath()
        path.move(to: strokeStart)

        if isOneOrTwoDots {
            path.addLine(to: strokeEnd)
        } else {
            for dot in dotsArrayToUse {
                path.addLine(to: self.convertDotToCGPoint(scene: scene, dot: dot))
            }
            path.addLine(to: strokeEnd)
        }
        return path
    }

    private func convertDotToCGPoint(scene: StrokeEditorScene, dot: IncomingDots) -> CGPoint {
        let dot = CGPoint(x: dot.x, y: dot.y)
        return scene.convertPoint(fromView: dot)
    }
}
