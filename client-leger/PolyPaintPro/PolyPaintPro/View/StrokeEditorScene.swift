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
    func sendEditorAction(actionId: Int, strokeUuid: String = "") {
        // Only send if the socket is connected
        if SocketManager.sharedInstance.getConnectionStatus() {
            do {
                let builtAction = BuildStrokeActionStrategyContext(scene: self, actionId: actionId, strokeUuid: strokeUuid)
                let outgoingActionMessage = builtAction.getOutgoingMessage()
                let encodedData = try JSONEncoder().encode(outgoingActionMessage)
                SocketManager.sharedInstance.send(data: encodedData)

                // Special case: We call our local reset here to prevent an infinite loop.
                if actionId == StrokeActionIdConstants.reset.rawValue {
                    self.resetCanvas()
                }
            } catch let error {
                print(error)
            }
        }
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
        self.sendEditorAction(actionId: StrokeActionIdConstants.add.rawValue, strokeUuid: shapeNode.id)
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
                self.sendEditorAction(actionId: StrokeActionIdConstants.replace.rawValue, strokeUuid: stroke.id)
                stroke.removeFromParent()
            }
        }
    }

    func eraseByStrokeWith(strokeUuid: String) {
        // We're looking for the right stroke to erase...

        // ... in our local strokes list ...
        enumerateChildNodes(withName: self.COMPLETESTROKE, using: {node, stop in
            let currentNode = node as! SKStroke
            if currentNode.id == strokeUuid {
                node.removeFromParent()
            }
        })

        // ... and in our online strokes list.
        enumerateChildNodes(withName: self.RECEIVEDSTROKE, using: {node, stop in
            let currentNode = node as! SKStroke
            if currentNode.id == strokeUuid {
                node.removeFromParent()
            }
        })
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
