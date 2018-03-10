//
//  StrokeEditorScene.swift
//  PolyPaintPro
//
//  Created by Kenny Nguyen on 2018-03-09.
//  Copyright © 2018 Les Pods c'est pour les lunchs. All rights reserved.
//

import SpriteKit

struct Stack {
    fileprivate var array: [SKShapeNode] = []
    mutating func push(_ element: SKShapeNode) {
        array.append(element)
    }

    mutating func pop() -> SKShapeNode? {
        return array.popLast()
    }

    func peek() -> SKShapeNode? {
        return array.last
    }

    func isEmpty() -> Bool {
        return array.isEmpty
    }
}

class StrokeEditorScene: SKScene {
    let INCOMPLETESTROKE = "line"
    let COMPLETESTROKE = "stroke"

    var strokesStack: Stack = Stack()
    var wayPoints: [CGPoint] = []
    var continuousStroke = false
    var nStrokes = 0
    var start = CGPoint.zero
    var end = CGPoint.zero
    var lastLocation = CGPoint.zero

    override func didMove(to view: SKView) {
        self.backgroundColor = SKColor.white
        self.yScale = -1
    }

    override func touchesBegan(_ touches: Set<UITouch>, with event: UIEvent?) {
        self.continuousStroke = false
        if let touch = touches.first as? UITouch {
            self.start = touch.location(in: self)
            self.lastLocation = start
            self.addPoint(point: self.lastLocation)
        }
    }

    override func touchesMoved(_ touches: Set<UITouch>, with event: UIEvent?) {
        continuousStroke = true
        if let touch = touches.first as? UITouch {
            let currentLocation = touch.location(in: self)
            print("this is SpriteKit")
            print(currentLocation.x)
            print(currentLocation.y)
            self.addPoint(point: currentLocation)
            self.drawLines(start: self.lastLocation, end: currentLocation)
            self.lastLocation = currentLocation
        }
    }

    override func touchesEnded(_ touches: Set<UITouch>, with event: UIEvent?) {
        self.nStrokes += 1
        if !continuousStroke {
            self.drawStroke(start: self.start, end: self.start)
        } else {
            if let touch = touches.first as? UITouch {
                self.end = touch.location(in: self)
                // we want to remove all the intermediate lines used to draw the stroke
                enumerateChildNodes(withName: self.INCOMPLETESTROKE, using: {node, stop in
                    node.removeFromParent()
                })
                // we redraw the stroke using all the collected waypoints
                self.drawStroke(start: self.start, end: self.end)
            }
        }
        // resets all values for the next stroke
        self.resetStrokeValues()
    }

    func clearWaypoints() {
        wayPoints.removeAll(keepingCapacity: false)
    }

    func resetStrokeValues() {
        self.clearWaypoints()
        self.start = CGPoint.zero
        self.end = CGPoint.zero
        self.lastLocation = CGPoint.zero
    }

    func resetCanvas() {
        enumerateChildNodes(withName: self.COMPLETESTROKE, using: {node, stop in
            node.removeFromParent()
        })
        self.nStrokes = 0
        self.resetStrokeValues()
    }

    func addPoint(point: CGPoint) {
        wayPoints.append(point)
    }

    //this serves as a preview for the stroke that's currently drawn
    func drawLines(start: CGPoint, end: CGPoint) {
        let path = CGMutablePath()

        path.move(to: start)
        path.addLine(to: end)

        let shapeNode = SKShapeNode()
        shapeNode.path = path
        shapeNode.name = self.INCOMPLETESTROKE
        shapeNode.strokeColor = UIColor.gray
        shapeNode.lineWidth = 10
        shapeNode.lineJoin = CGLineJoin.round
        shapeNode.lineCap = CGLineCap.round

        self.addChild(shapeNode)
    }

    // this creates a stroke that is stored
    func drawStroke(start: CGPoint, end: CGPoint) {
        let path = CGMutablePath()

        path.move(to: start)
        for point in self.wayPoints {
            path.addLine(to: point)
        }
        path.addLine(to: end)

        let shapeNode = SKShapeNode()
        shapeNode.path = path
        shapeNode.name = self.COMPLETESTROKE
        shapeNode.strokeColor = UIColor.gray
        shapeNode.lineWidth = 10
        //shapeNode.lineJoin = CGLineJoin.round
        shapeNode.lineCap = CGLineCap.round

        self.addChild(shapeNode)
    }

    // removes the child from view and adds it to the stack
    func stack() {
        if (!self.children.isEmpty) {
            let lastStroke = self.children.last as! SKShapeNode
            self.strokesStack.push(lastStroke)
            lastStroke.removeFromParent()
        } else {
            // TO-DO : Show an error message?
        }
    }
    // adds the child to the view and removes it from the stack
    func unstack() {
        if (!self.strokesStack.isEmpty()) {
            let stroke: SKShapeNode = self.strokesStack.pop()!
            self.addChild(stroke)
        } else {
            // TO-DO : Show an error message?
        }
    }
}
