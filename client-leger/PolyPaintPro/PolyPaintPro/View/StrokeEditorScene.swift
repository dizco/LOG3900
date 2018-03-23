//
//  StrokeEditorScene.swift
//  PolyPaintPro
//
//  Created by Kenny Nguyen on 2018-03-09.
//  Copyright © 2018 Les Pods c'est pour les lunchs. All rights reserved.
//

import SpriteKit

struct Stack {
    fileprivate var array: [SKStroke] = []
    mutating func push(_ element: SKStroke) {
        array.append(element)
    }

    mutating func pop() -> SKStroke? {
        return array.popLast()
    }

    func peek() -> SKStroke? {
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
    // MARK: - Constants
    internal let INCOMPLETESTROKE = "line"
    internal let COMPLETESTROKE = "stroke"
    internal let RECEIVEDSTROKE = "receivedstroke"

    // MARK: - View Size
    var skViewSize = CGSize.zero

    // MARK: - Strokes drawing parameters
    internal var red: CGFloat = 0.0
    internal var green: CGFloat = 0.0
    internal var blue: CGFloat = 0.0
    internal var opacity: CGFloat = 1.0
    internal var width: CGFloat = 10.0

    // MARK: - Strokes parameters
    internal var strokesStack: Stack = Stack()
    internal var wayPoints: [CGPoint] = []
    internal var nStrokes = 0
    internal var start = CGPoint.zero
    internal var end = CGPoint.zero
    internal var lastLocation = CGPoint.zero

    // MARK: - Editing mode
    internal var currentEditingMode = EditingMode.ink // will be used to switch editing modes

    override func didMove(to view: SKView) {
        self.backgroundColor = SKColor.white
    }

    func setEditingMode(mode: EditingMode) {
        switch mode {
        case .ink:
            self.currentEditingMode = EditingMode.ink
        case .select:
            self.currentEditingMode = EditingMode.select
        case .eraseByPoint:
            self.currentEditingMode = EditingMode.eraseByPoint
        case .eraseByStroke:
            self.currentEditingMode = EditingMode.eraseByStroke
        }
    }

    // MARK: - Get the view size
    func saveCurrentViewSize(size: CGSize) {
        self.skViewSize = size
    }

    // MARK: - Called by DrawingToolsViewDelegate
    func updateColorValues(red: Int, green: Int, blue: Int, opacity: Int) {
        self.red = CGFloat(red) / 255.0
        self.green = CGFloat(green) / 255.0
        self.blue = CGFloat(blue) / 255.0
        self.opacity = CGFloat(opacity) / 255.0
    }

    func updateBrushSize(size: Int) {
        self.width = CGFloat(size)
    }

    // MARK: - Functions for received actions
    func applyReceived(incomingAction: IncomingActionMessage) {
        if incomingAction.type == IncomingMessageConstants.strokeAction.rawValue {
            for receivedStroke in incomingAction.delta.add {
                let path = self.createReceivedPathWith(dotsArray: receivedStroke.dots)
                self.drawReceived(stroke: receivedStroke)
            }
        }
    }

    // IncomingAdd is the struct that contains the info of the stroke to draw
    private func drawReceived(stroke: IncomingAdd) {
        let path = self.createReceivedPathWith(dotsArray: stroke.dots)
        let color = self.convertHexToUIColor(hex: stroke.strokeAttributes.color)!

        let shapeNode = SKStroke()
        shapeNode.path = path
        shapeNode.name = self.RECEIVEDSTROKE
        shapeNode.setReceivedUuid(uuid: stroke.strokeUuid)

        shapeNode.strokeColor = color
        shapeNode.lineWidth = CGFloat(stroke.strokeAttributes.width)
        // Can't do stuff with strokeAttributes.height
        shapeNode.lineJoin = CGLineJoin.round
        shapeNode.lineCap = CGLineCap.round

        self.addChild(shapeNode)
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
        let length = hexSanitized.characters.count

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

    private func createReceivedPathWith(dotsArray: [IncomingDots]) -> CGMutablePath {
        let strokeStart: CGPoint
        let strokeEnd: CGPoint
        var dotsArrayToUse: [IncomingDots] = dotsArray
        var isOneOrTwoDots = false

        if dotsArray.count == 1 {
            isOneOrTwoDots = true
            strokeStart = self.convertDotToCGPoint(dot: dotsArray.first!)
            strokeEnd = strokeStart
        } else if dotsArray.count == 2 {
            isOneOrTwoDots = true
            strokeStart = self.convertDotToCGPoint(dot: dotsArray.first!)
            strokeEnd = self.convertDotToCGPoint(dot: dotsArray.last!)
        } else {
            strokeStart = self.convertDotToCGPoint(dot: dotsArray.first!)
            strokeEnd = self.convertDotToCGPoint(dot: dotsArray.last!)
            let tmpDotsArray = dotsArray.dropFirst()
            dotsArrayToUse = Array(tmpDotsArray.dropLast())
        }

        let path = CGMutablePath()
        path.move(to: strokeStart)

        if isOneOrTwoDots {
            path.addLine(to: strokeEnd)
        } else {
            for dot in dotsArrayToUse {
                path.addLine(to: self.convertDotToCGPoint(dot: dot))
            }
            path.addLine(to: strokeEnd)
        }
        return path
    }

    private func convertDotToCGPoint(dot: IncomingDots) -> CGPoint {
        let dot = CGPoint(x: dot.x, y: dot.y)
        return self.convertPoint(fromView: dot)
    }

    // MARK: - Touches function
    override func touchesBegan(_ touches: Set<UITouch>, with event: UIEvent?) {
        print("touches began")
        switch self.currentEditingMode {
        case .ink:
            if let touch = touches.first as? UITouch {
                self.start = touch.location(in: self)
                self.lastLocation = start
                self.addPoint(point: self.lastLocation)
            }
        case .select:
            print("Not implemented.")
        case .eraseByPoint:
            print("Not implemented.")
        case .eraseByStroke:
            if let touch = touches.first as? UITouch {
                self.start = touch.location(in: self)
                self.eraseByStroke(position: self.start)
            }
        }
    }

    override func touchesMoved(_ touches: Set<UITouch>, with event: UIEvent?) {
        switch self.currentEditingMode {
        case .ink:
            if let touch = touches.first as? UITouch {
                let currentLocation = touch.location(in: self)
                print("x", currentLocation.x)
                print("y", currentLocation.y)
                self.addPoint(point: currentLocation)
                self.drawLines(start: self.lastLocation, end: currentLocation)
                self.lastLocation = currentLocation
            }
        case .select:
            print("Not implemented.")
        case .eraseByPoint:
            print("Not implemented.")
        case .eraseByStroke:
            if let touch = touches.first as? UITouch {
                let currentPosition = touch.location(in: self)
                self.eraseByStroke(position: currentPosition)
            }
        }

    }

    override func touchesEnded(_ touches: Set<UITouch>, with event: UIEvent?) {
        self.nStrokes += 1 // this is used to test currently, will be removed later

        print("touches ended")
        switch self.currentEditingMode {
        case .ink:
            if let touch = touches.first as? UITouch {
                self.end = touch.location(in: self)
                // we want to remove all the intermediate lines used to draw the stroke
                enumerateChildNodes(withName: self.INCOMPLETESTROKE, using: {node, stop in
                    node.removeFromParent()
                })
                // we redraw the stroke using all the collected waypoints
                self.drawStroke(start: self.start, end: self.end)
            }
        case .select:
            print("Not implemented.")
        case .eraseByPoint:
            print("Not implemented.")
        case .eraseByStroke:
            if let touch = touches.first as? UITouch {
                let currentPosition = touch.location(in: self)
                self.eraseByStroke(position: currentPosition)
            }
        }

        // resets all values for the next stroke
        self.resetStrokeValues()
    }

    override func touchesCancelled(_ touches: Set<UITouch>, with event: UIEvent?) {
        // this function detects "taps" where the user briefly touches somewhere and releases his finger
        print("touches cancel")
        switch self.currentEditingMode {
        case .ink:
            self.drawStroke(start: self.start, end: self.start)
        case .select:
            print("Not implemented.")
        case .eraseByPoint:
            print("Not implemented.")
        case .eraseByStroke:
            self.eraseByStroke(position: self.start)
        }

        // resets all values for the next stroke
        self.resetStrokeValues()
    }

    // MARK: - Class functions
    private func clearWaypoints() {
        wayPoints.removeAll(keepingCapacity: false)
    }

    private func resetStrokeValues() {
        self.clearWaypoints()
        self.start = CGPoint.zero
        self.end = CGPoint.zero
        self.lastLocation = CGPoint.zero
    }

    func resetCanvas() {
        enumerateChildNodes(withName: self.COMPLETESTROKE, using: {node, stop in
            node.removeFromParent()
        })
        enumerateChildNodes(withName: self.RECEIVEDSTROKE, using: {node, stop in
            node.removeFromParent()
        })
        self.nStrokes = 0
        self.resetStrokeValues()
    }

    private func addPoint(point: CGPoint) {
        wayPoints.append(point)
    }

    //this serves as a preview for the stroke that's currently drawn
    private func drawLines(start: CGPoint, end: CGPoint) {
        let path = CGMutablePath()

        path.move(to: start)
        path.addLine(to: end)

        let shapeNode = SKShapeNode()
        shapeNode.path = path
        shapeNode.name = self.INCOMPLETESTROKE
        shapeNode.strokeColor = UIColor(red: self.red, green: self.green, blue: self.blue, alpha: self.opacity)
        shapeNode.lineWidth = self.width
        shapeNode.lineJoin = CGLineJoin.round
        shapeNode.lineCap = CGLineCap.round

        self.addChild(shapeNode)
    }

    // this creates a stroke that is stored
    private func drawStroke(start: CGPoint, end: CGPoint) {
        let path = CGMutablePath()

        path.move(to: start)
        for point in self.wayPoints {
            path.addLine(to: point)
        }
        path.addLine(to: end)

        let shapeNode = SKStroke()
        shapeNode.path = path
        shapeNode.name = self.COMPLETESTROKE
        shapeNode.strokeColor = UIColor(red: self.red, green: self.green, blue: self.blue, alpha: self.opacity)
        shapeNode.lineWidth = self.width
        shapeNode.lineCap = CGLineCap.round

        self.addChild(shapeNode)
    }

    private func eraseByStroke(position: CGPoint) {
        // most recent stroke is returned as the first one
        let strokeToBeErased = self.nodes(at: position).first as? SKStroke
        let strokesToBeErased = self.nodes(at: position) as? [SKStroke]

        if strokesToBeErased == nil {
            return
        } else {
            print(strokesToBeErased!)
        }

        for stroke in strokesToBeErased! {
            if stroke.contains(position) {
                stroke.removeFromParent()
            }
        }

    }

    func eraseByPoint(position: CGPoint) {
        let strokeToBeErased = self.nodes(at: position).first as? SKStroke
        let pointsList = strokeToBeErased?.path?.getPathElementsPoints()

        // TO-DO : Add a function to preview the eraser's path.....
        // TO-DO : Create a stroke with its initial starting point to the position where the eraser went

        // TO-DO : Create a stroke with the position where the eraser went to its initial ending point

        // TO-DO : Add them as children.
    }

    // removes the child from view and adds it to the stack
    func stack() {
        if !self.children.isEmpty {
            let lastStroke = self.children.last as! SKStroke
            self.strokesStack.push(lastStroke)
            lastStroke.removeFromParent()
        } else {
            // TO-DO : Disable the button
        }
    }

    // adds the child to the view and removes it from the stack
    func unstack() {
        if !self.strokesStack.isEmpty() {
            let stroke: SKStroke = self.strokesStack.pop()!
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
        var arrayPoints: [CGPoint]! = [CGPoint]()
        var arrayTypes: [CGPathElementType]! = [CGPathElementType]()
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
