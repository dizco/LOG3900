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

enum EditingMode {
    case ink, select, eraseByPoint, eraseByStroke
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
    var currentEditingMode = EditingMode.ink // will be used to switch editing modes

    override func didMove(to view: SKView) {
        self.backgroundColor = SKColor.white
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
        self.nStrokes += 1 // this is used to test currently, will be removed later
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

        // this is used to test the eraser
        /*
        if nStrokes == 3 {
            self.eraseByStroke(position: self.start)
        }*/
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
        // used to test the eraser
        /*
        if nStrokes == 2 {
            shapeNode.strokeColor = UIColor.cyan
            print("this is blue", UIColor.cyan)
        }
        if nStrokes == 3 {
            shapeNode.strokeColor = UIColor.red
            print("this is red", UIColor.red)
        }*/
        shapeNode.lineWidth = 10
        //shapeNode.lineJoin = CGLineJoin.round
        shapeNode.lineCap = CGLineCap.round

        self.addChild(shapeNode)
    }

    func eraseByStroke(position: CGPoint) {
        // most recent stroke is returned as the first one
        let strokeToBeErased = self.nodes(at: position).first as! SKShapeNode
        strokeToBeErased.removeFromParent()
    }

    func eraseByPoint(position: CGPoint) {
        let strokeToBeErased = self.nodes(at: position).first as! SKShapeNode
        let pointsList = strokeToBeErased.path!.getPathElementsPoints()

        // TO-DO : Add a function to preview the eraser's path.....
        // TO-DO : Create a stroke with its initial starting point to the position where the eraser went

        // TO-DO : Create a stroke with the position where the eraser went to its initial ending point

        // TO-DO : Add them as children.
    }

    // removes the child from view and adds it to the stack
    func stack() {
        if !self.children.isEmpty {
            let lastStroke = self.children.last as! SKShapeNode
            self.strokesStack.push(lastStroke)
            lastStroke.removeFromParent()
        } else {
            // TO-DO : Disable the button
        }
    }
    // adds the child to the view and removes it from the stack
    func unstack() {
        if !self.strokesStack.isEmpty() {
            let stroke: SKShapeNode = self.strokesStack.pop()!
            self.addChild(stroke)
        } else {
            // TO-DO : Disable the button
        }
    }
}
/* https://stackoverflow.com/questions/12992462/how-to-get-the-cgpoints-of-a-cgpath */
// work-in-progress for eraseByPoint
extension CGPath {

    func forEach( body: @convention(block) (CGPathElement) -> Void) {
        typealias Body = @convention(block) (CGPathElement) -> Void
        let callback: @convention(c) (UnsafeMutableRawPointer, UnsafePointer<CGPathElement>) -> Void = { (info, element) in
            let body = unsafeBitCast(info, to: Body.self)
            body(element.pointee)
        }
        print(MemoryLayout.size(ofValue: body))
        let unsafeBody = unsafeBitCast(body, to: UnsafeMutableRawPointer.self)
        self.apply(info: unsafeBody, function: unsafeBitCast(callback, to: CGPathApplierFunction.self))
    }


    func getPathElementsPoints() -> [CGPoint] {
        var arrayPoints : [CGPoint]! = [CGPoint]()
        self.forEach { element in
            switch (element.type) {
            case CGPathElementType.moveToPoint:
                arrayPoints.append(element.points[0])
            case .addLineToPoint:
                arrayPoints.append(element.points[0])
            case .addQuadCurveToPoint:
                arrayPoints.append(element.points[0])
                arrayPoints.append(element.points[1])
            case .addCurveToPoint:
                arrayPoints.append(element.points[0])
                arrayPoints.append(element.points[1])
                arrayPoints.append(element.points[2])
            default: break
            }
        }
        return arrayPoints
    }

    func getPathElementsPointsAndTypes() -> ([CGPoint],[CGPathElementType]) {
        var arrayPoints : [CGPoint]! = [CGPoint]()
        var arrayTypes : [CGPathElementType]! = [CGPathElementType]()
        self.forEach { element in
            switch (element.type) {
            case CGPathElementType.moveToPoint:
                arrayPoints.append(element.points[0])
                arrayTypes.append(element.type)
            case .addLineToPoint:
                arrayPoints.append(element.points[0])
                arrayTypes.append(element.type)
            case .addQuadCurveToPoint:
                arrayPoints.append(element.points[0])
                arrayPoints.append(element.points[1])
                arrayTypes.append(element.type)
                arrayTypes.append(element.type)
            case .addCurveToPoint:
                arrayPoints.append(element.points[0])
                arrayPoints.append(element.points[1])
                arrayPoints.append(element.points[2])
                arrayTypes.append(element.type)
                arrayTypes.append(element.type)
                arrayTypes.append(element.type)
            default: break
            }
        }
        return (arrayPoints,arrayTypes)
    }
}
