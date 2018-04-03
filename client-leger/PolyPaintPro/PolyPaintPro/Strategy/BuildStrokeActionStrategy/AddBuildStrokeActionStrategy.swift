//
//  AddBuildStrokeActionStrategy.swift
//  PolyPaintPro
//
//  Created by Kenny Nguyen on 2018-04-03.
//  Copyright © 2018 Les Pods c'est pour les lunchs. All rights reserved.
//

import Foundation
import SpriteKit

final class AddBuildStrokeActionStrategy: BuildStrokeActionStrategy {
    func buildOutgoingAction(scene: StrokeEditorScene, actionId: Int, strokeUuid: String) -> OutgoingActionMessage {

        // 1. Convert the waypoints into dots
        let dots: [OutgoingDots] = convertWaypointsToDots(scene: scene)
        // 2. Save the stroke attributes
        let strokeAttributes: OutgoingStrokeAttributes = buildStrokeAttributes(scene: scene)
        // 3. Combine 1 and 3 into an OutgoingAdd
        var add: [OutgoingAdd] = []
        let stroke = OutgoingAdd(strokeUuid: strokeUuid, strokeAttributes: strokeAttributes, dots: dots)
        add.append(stroke)

        // 4. Create the OutgoingDelta
        let delta: OutgoingDelta = OutgoingDelta(add: add)

        // 5. Create the OutgoingActionMessage
        return OutgoingActionMessage(actionId: actionId, actionName: StrokeActionNameConstants.addActionName.rawValue, delta: delta)
    }

    private func buildStrokeAttributes(scene: StrokeEditorScene) -> OutgoingStrokeAttributes {
        let color = convertUIColorToHex(scene: scene)!
        return OutgoingStrokeAttributes(color: color, height: Int(scene.width), width: Int(scene.width))
    }

    private func convertUIColorToHex(scene: StrokeEditorScene, withAlpha: Bool = true) -> String? {
        // https://cocoacasts.com/from-hex-to-uicolor-and-back-in-swift
        let red = Float(scene.red)
        let green = Float(scene.green)
        let blue = Float(scene.blue)
        let alpha = Float(scene.alphaValue)

        // swiftlint:disable line_length
        if withAlpha {
            return String(format: "#%02lX%02lX%02lX%02lX", lroundf(alpha * 255), lroundf(red * 255), lroundf(green * 255), lroundf(blue * 255))
        } else {
            return String(format: "#%02lX%02lX%02lX", lroundf(red * 255), lroundf(green * 255), lroundf(blue * 255))
        }
        // swiftlint:enable line_length
    }

    private func convertWaypointsToDots(scene: StrokeEditorScene) -> [OutgoingDots] {

        var dots: [OutgoingDots] = []

        // Convert the start point into server coordinates
        let startPoint = convertCGPointToDot(scene: scene, point: scene.start)
        let start = OutgoingDots(x: Double(startPoint.x), y: Double(startPoint.y))

        // Convert the end point into server coordinates
        let endPoint = convertCGPointToDot(scene: scene, point: scene.end)
        let end = OutgoingDots(x: Double(endPoint.x), y: Double(endPoint.y))

        dots.append(start)
        // Convert the rest of the points into server coordinates
        for wayPoint in scene.wayPoints {
            let point = self.convertCGPointToDot(scene: scene, point: wayPoint)
            let dot = OutgoingDots(x: Double(point.x), y: Double(point.y))
            dots.append(dot)
        }

        // if it's only a dot
        if scene.wayPoints.count > 1 {
            dots.append(end)
        }
        return dots
    }

    private func convertCGPointToDot(scene: StrokeEditorScene, point: CGPoint) -> CGPoint {
        return scene.convertPoint(toView: point)
    }
}
