//
//  SKStroke.swift
//  PolyPaintPro
//
//  Created by Kenny Nguyen on 2018-03-20.
//  Copyright © 2018 Les Pods c'est pour les lunchs. All rights reserved.
//

import Foundation
import SpriteKit
import AVFoundation

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
    // MARK: - Stroke creation parameters
    var wayPoints: [CGPoint] = []
    var start: CGPoint = CGPoint.zero
    var end: CGPoint = CGPoint.zero

    var red: CGFloat = 0.0
    var green: CGFloat = 0.0
    var blue: CGFloat = 0.0
    var alphaValue: CGFloat = 1.0

    var width: CGFloat = 10.0

    var id: String = UUID().uuidString.lowercased()

    // MARK: - Parameter used for EraseByPoint
    var dots: [CGPoint]?

    // MARK: - Parameter used for Lock/Unlock Stroke action
    var isLocked: Bool = false

    // MARK: - Parameter used as a band-aid fix for not erasing the same stroke twice
    var isErased: Bool = false

    // MARK: - Class functions
    func saveParameters(color: SKStrokeColor, dots: SKStrokeDots, width: CGFloat) {
        self.red = color.red
        self.green = color.green
        self.blue = color.blue
        self.alphaValue = color.alphaValue

        self.wayPoints = dots.wayPoints
        self.start = dots.start
        self.end = dots.end

        self.width = width
    }

    // Function used to add precision to EraseByPoint
    func generateDotsFromPath() {
        if self.dots == nil {
            self.dots = self.path?.getPathElementsPoints()
        }
    }

    func setReceivedUuid(uuid: String) {
        self.id = uuid
    }

    func isCloseTo(position: CGPoint) -> Bool {
        // Related to EraseByStroke
        if self.isLocked {
            self.playFeedbackIfLocked()
            return false
        }

        if self.isErased {
            // it's already erased so we don't want to repeat the action
            // related to the bug where audio feedback plays more than once when we go over the stroke
            // also related to the bug where we send the stroke to be erased twice or more
            return false
        }

        let padding: CGFloat = 10.0 + self.width

        let lowerBoundX: CGFloat = position.x - padding
        let upperBoundX: CGFloat = position.x + padding

        let lowerBoundY: CGFloat = position.y - padding
        let upperBoundY: CGFloat = position.y + padding

        let pointsList = self.path?.getPathElementsPoints()

        for point in pointsList! {
            if lowerBoundX <= point.x && point.x <= upperBoundX && lowerBoundY <= point.y && point.y <= upperBoundY {
                self.isErased = true
                return true
            }
        }
        return false
    }

    func splitSelf(position: CGPoint) -> [SKStroke] {
        var newStrokes: [SKStroke] = []

        // Related to EraseByPoint
        if self.isLocked {
            self.playFeedbackIfLocked()
            return newStrokes
        }

        // This padding value works very well
        let padding: CGFloat = self.width / 1.5

        let lowerBoundX: CGFloat = position.x - padding
        let upperBoundX: CGFloat = position.x + padding

        let lowerBoundY: CGFloat = position.y - padding
        let upperBoundY: CGFloat = position.y + padding

        for (index, point) in self.dots!.enumerated() {
            if lowerBoundX <= point.x && point.x <= upperBoundX && lowerBoundY <= point.y && point.y <= upperBoundY {
                // Start of a stroke
                if index == 0 {
                    let strokePts = Array(self.dots![1 ..< self.dots!.endIndex])
                    let stroke = self.createNewStroke(wayPoints: strokePts)

                    newStrokes.append(stroke)

                    return newStrokes

                // End of a stroke
                } else if index == self.dots!.endIndex - 1 {
                    let strokePts = Array(self.dots![0 ..< self.dots!.endIndex - 1])
                    let stroke = self.createNewStroke(wayPoints: strokePts)

                    newStrokes.append(stroke)

                // Middle of a stroke
                } else {
                    let strokePts1 = Array(self.dots![0 ..< index])
                    let strokePts2 = Array(self.dots![index ..< self.dots!.endIndex])

                    let stroke1 = self.createNewStroke(wayPoints: strokePts1)
                    let stroke2 = self.createNewStroke(wayPoints: strokePts2)

                    newStrokes.append(stroke1)
                    newStrokes.append(stroke2)

                    return newStrokes
                }
            }
        }
        return newStrokes
    }

    private func createNewStroke(wayPoints: [CGPoint]) -> SKStroke {
        let path = CGMutablePath()

        for point in wayPoints {
            if point == wayPoints.first {
                path.move(to: point)
            } else {
                path.addLine(to: point)
            }
        }

        // Create the stroke
        let shapeNode = SKStroke()
        shapeNode.path = path
        shapeNode.name = "stroke"
        shapeNode.strokeColor = UIColor(red: self.red, green: self.green, blue: self.blue, alpha: self.alphaValue)
        shapeNode.lineWidth = self.width
        shapeNode.lineCap = CGLineCap.round

        // Save the stroke parameters in its own class
        let strokeColor = SKStrokeColor(red: self.red, green: self.green, blue: self.blue, alpha: self.alphaValue)
        let strokeDots = SKStrokeDots(wayPoints: wayPoints, start: wayPoints.first!, end: wayPoints.last!)
        shapeNode.saveParameters(color: strokeColor, dots: strokeDots, width: self.width)
        shapeNode.generateDotsFromPath()

        return shapeNode
    }

    func lock() {
        self.isLocked = true
    }

    func unlock() {
        self.isLocked = false
    }

    private func playFeedbackIfLocked() {
        let systemSoundID: SystemSoundID = 1107
        AudioServicesPlaySystemSound(systemSoundID)

        // Animate the stroke that is locked
        let fadeOut = SKAction.fadeOut(withDuration: 0.1)
        let fadeIn = SKAction.fadeIn(withDuration: 0.1)
        let blink = SKAction.sequence([fadeOut, fadeIn])
        let blinkAction = SKAction.repeat(blink, count: 2)
        self.run(blinkAction)
    }
}
