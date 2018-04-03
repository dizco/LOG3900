//
//  StrokeEditorScene.swift
//  PolyPaintPro
//
//  Created by Kenny Nguyen on 2018-03-09.
//  Copyright © 2018 Les Pods c'est pour les lunchs. All rights reserved.
//

import SpriteKit

internal struct Stack {
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

enum StrokeEditingMode {
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
    internal var alphaValue: CGFloat = 1.0
    internal var width: CGFloat = 10.0

    // MARK: - Strokes parameters
    internal var strokesStack: Stack = Stack()
    internal var wayPoints: [CGPoint] = []
    internal var nStrokes = 0
    internal var start = CGPoint.zero
    internal var end = CGPoint.zero
    internal var lastLocation = CGPoint.zero

    // MARK: - Editing mode
    internal var currentEditingMode = StrokeEditingMode.ink // will be used to switch editing modes

    override func didMove(to view: SKView) {
        self.backgroundColor = SKColor.white
    }

    func setEditingMode(mode: StrokeEditingMode) {
        self.currentEditingMode = mode
    }

    // MARK: - Get the view size
    func saveCurrentViewSize(size: CGSize) {
        self.skViewSize = size
    }

    // MARK: - Called by DrawingToolsViewDelegate
    func updateColorValues(red: Int, green: Int, blue: Int, alpha: Int) {
        self.red = CGFloat(red) / 255.0
        self.green = CGFloat(green) / 255.0
        self.blue = CGFloat(blue) / 255.0
        self.alphaValue = CGFloat(alpha) / 100.0
    }

    func updateBrushSize(size: Int) {
        self.width = CGFloat(size)
    }

    // MARK: - Functions for sending strokes
    func buildOutgoingAction(actionId: Int, strokeUuid: String) -> OutgoingActionMessage? {

        // 1. Convert the waypoints into dots
        let dots: [OutgoingDots] = convertWaypointsToDots()
        // 2. Save the stroke attributes
        let strokeAttributes: OutgoingStrokeAttributes = buildStrokeAttributes()
        // 3. Combine 1 and 3 into an OutgoingAdd
        var add: [OutgoingAdd] = []
        let stroke = OutgoingAdd(strokeUuid: strokeUuid, strokeAttributes: strokeAttributes, dots: dots)
        add.append(stroke)

        // 4. Create the OutgoingDelta
        // TO-DO : Differentiate between adding a new stroke and removing
        let delta: OutgoingDelta = OutgoingDelta(add: add)

        // 5. Create the OutgoingActionMessage
        if actionId == 1 {
            return OutgoingActionMessage(actionId: actionId, actionName: "NewStroke", delta: delta)
        } else {
            return nil
        }
    }

    private func buildStrokeAttributes() -> OutgoingStrokeAttributes {
        let color = convertUIColorToHex()!
        return OutgoingStrokeAttributes(color: color, height: Int(self.width), width: Int(self.width))
    }

    private func convertUIColorToHex(withAlpha: Bool = true) -> String? {
        // https://cocoacasts.com/from-hex-to-uicolor-and-back-in-swift
        let red = Float(self.red)
        let green = Float(self.green)
        let blue = Float(self.blue)
        let alpha = Float(self.alphaValue)

        // swiftlint:disable line_length
        if withAlpha {
            return String(format: "#%02lX%02lX%02lX%02lX", lroundf(alpha * 255), lroundf(red * 255), lroundf(green * 255), lroundf(blue * 255))
        } else {
            return String(format: "#%02lX%02lX%02lX", lroundf(red * 255), lroundf(green * 255), lroundf(blue * 255))
        }
        // swiftlint:enable line_length
    }

    private func convertWaypointsToDots() -> [OutgoingDots] {

        var dots: [OutgoingDots] = []

        // Convert the start point into server coordinates
        let startPoint = convertCGPointToDot(point: self.start)
        let start = OutgoingDots(x: Double(startPoint.x), y: Double(startPoint.y))

        // Convert the end point into server coordinates
        let endPoint = convertCGPointToDot(point: self.end)
        let end = OutgoingDots(x: Double(endPoint.x), y: Double(endPoint.y))

        dots.append(start)
        // Convert the rest of the points into server coordinates
        for wayPoint in self.wayPoints {
            let point = self.convertCGPointToDot(point: wayPoint)
            let dot = OutgoingDots(x: Double(point.x), y: Double(point.y))
            dots.append(dot)
        }

        // if it's only a dot
        if self.wayPoints.count > 1 {
            dots.append(end)
        }
        return dots
    }

    private func convertCGPointToDot(point: CGPoint) -> CGPoint {
        return self.convertPoint(toView: point)
    }

    // MARK: - Function for received actions
    func applyReceived(incomingAction: IncomingActionMessage) {
        _ = StrokeActionStrategyContext(scene: self, incomingAction: incomingAction)
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
        shapeNode.strokeColor = UIColor(red: self.red, green: self.green, blue: self.blue, alpha: self.alphaValue)
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

        // Create the stroke to be added to canvas
        let shapeNode = SKStroke()
        shapeNode.path = path
        shapeNode.name = self.COMPLETESTROKE
        shapeNode.strokeColor = UIColor(red: self.red, green: self.green, blue: self.blue, alpha: self.alphaValue)
        shapeNode.lineWidth = self.width
        shapeNode.lineCap = CGLineCap.round

        // Save the stroke parameters in its own class
        let strokeColor = SKStrokeColor(red: self.red, green: self.green, blue: self.blue, alpha: self.alphaValue)
        let strokeDots = SKStrokeDots(wayPoints: self.wayPoints, start: start, end: end)
        shapeNode.saveParameters(color: strokeColor, dots: strokeDots, width: self.width)

        self.addChild(shapeNode)

        // Only send the stroke if the socket is connected
        if SocketManager.sharedInstance.getConnectionStatus() {
            if self.currentEditingMode == StrokeEditingMode.ink {
                do {
                    let outgoingActionMessage = self.buildOutgoingAction(actionId: 1, strokeUuid: shapeNode.id)!
                    let encodedData = try JSONEncoder().encode(outgoingActionMessage)
                    SocketManager.sharedInstance.send(data: encodedData)
                } catch let error {
                    print(error)
                }
            }
        }
    }

    private func eraseByStroke(position: CGPoint) {
        // most recent stroke is returned as the first one
        let strokesToBeErased = self.nodes(at: position) as? [SKStroke]

        if strokesToBeErased == nil {
            return
        } else {
            print(strokesToBeErased!)
        }

        for stroke in strokesToBeErased! {
            if stroke.isCloseTo(position: position) {
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
