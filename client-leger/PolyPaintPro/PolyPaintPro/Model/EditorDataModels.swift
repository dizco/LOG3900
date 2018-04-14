//
//  ActionMessage.swift
//  PolyPaintPro
//
//  Created by Kenny Nguyen on 2018-03-20.
//  Copyright © 2018 Les Pods c'est pour les lunchs. All rights reserved.
//

import Foundation

// MARK: - Subscription models
struct DrawingSubscription: Codable {
    var type: String
    var action: ActionSubscription
    var drawing: OutgoingDrawing
    init() {
        self.type = "client.editor.subscription"
        self.action = ActionSubscription(id: "join", name: "")
        self.drawing = OutgoingDrawing(id: "5ab32000a65b33724fa923c5") // TO-DO : Generate a drawing id on join.
    }
}

struct ActionSubscription: Codable {
    let id: String
    let name: String
    init(id: String, name: String) {
        self.id = id
        self.name = name
    }
}

// MARK: - ActionMessage
protocol ActionMessage {
    var type: String { get }
    var action: Action { get }
}

protocol StringIdActionMessage {
    var type: String { get }
    var action: StringIdAction { get }
}

struct Action: Codable {
    let id: Int
    let name: String
    init(id: Int, name: String) {
        self.id = id
        self.name = name
    }
}

struct StringIdAction: Codable {
    let id: String
    let name: String
    init(id: String, name: String) {
        self.id = id
        self.name = name
    }
}

// MARK: - FOR BOTH EDITORS

struct IncomingDrawing: Codable {
    let id: String
    let name: String
    let mode: String
    let owner: IncomingOwner
    let protection: IncomingProtection
    var visibility: String
    let users: IncomingUsers
    let strokes: [IncomingStroke]
    let pixels: [IncomingPixels]

    enum CodingKeys: String, CodingKey {
        case id = "_id"
        case name
        case mode
        case owner
        case protection
        case visibility
        case users
        case strokes
        case pixels
    }
}

struct SubscriptionMessage: StringIdActionMessage, Codable {
    let type: String
    let action: StringIdAction
    let drawing: IncomingDrawingId

    init(actionId: String, actionName: String, drawingId: String) {
        self.type = OutgoingMessageConstants.subscription.rawValue
        self.action = StringIdAction(id: actionId, name: actionName)
        self.drawing = IncomingDrawingId(id: drawingId)
    }
}

// MARK: - FOR STROKE EDITOR

// MARK: - IncomingActionMessage - Stroke
struct IncomingActionMessage: ActionMessage, Codable {
    let type: String
    let action: Action
    let drawing: IncomingDrawingId
    let author: Author
    let delta: Delta
    let timestamp: Double
}

struct IncomingDrawingId: Codable {
    let id: String
}

struct IncomingProtection: Codable {
    let active: Bool
}

struct Delta: Codable {
    let add: [IncomingStroke]
    let remove: [String]
}

struct IncomingStroke: Codable {
    let strokeUuid: String
    let strokeAttributes: IncomingStrokeAttributes
    let dots: [IncomingDots]
}

struct IncomingStrokeAttributes: Codable {
    let color: String
    let height: Int
    let width: Int
    let stylusTip: String
}

struct IncomingDots: Codable {
    // swiftlint:disable identifier_name
    let x: Double
    let y: Double
    // swiftlint:enable identifier_name
}

// MARK: - OutgoingActionMessage - Stroke
struct OutgoingActionMessage: ActionMessage, Codable {
    let type: String
    let action: Action
    let drawing: OutgoingDrawing
    let delta: OutgoingDelta

    init(actionId: Int, actionName: String, drawingId: String, delta: OutgoingDelta) {
        self.type = OutgoingMessageConstants.strokeAction.rawValue
        self.action = Action(id: actionId, name: actionName)
        self.drawing = OutgoingDrawing(id: drawingId)
        self.delta = delta
    }
}

struct OutgoingDrawing: Codable {
    let id: String
    init(id: String) {
        self.id = id
    }
}

struct OutgoingDelta: Codable {
    let add: [OutgoingAdd]
    let remove: [String]
    init(add: [OutgoingAdd] = [], remove: [String] = []) {
        self.add = add
        self.remove = remove
    }
}

struct OutgoingAdd: Codable {
    let strokeUuid: String
    let strokeAttributes: OutgoingStrokeAttributes
    let dots: [OutgoingDots]
    init(strokeUuid: String, strokeAttributes: OutgoingStrokeAttributes, dots: [OutgoingDots]) {
        self.strokeUuid = strokeUuid
        self.strokeAttributes = strokeAttributes
        self.dots = dots
    }
}

struct OutgoingStrokeAttributes: Codable {
    let color: String
    let height: Int
    let width: Int
    let stylusTip: String = "Ellipse"
    init(color: String, height: Int, width: Int) {
        self.color = color
        self.height = height
        self.width = width
    }
}

struct OutgoingDots: Codable {
    // swiftlint:disable identifier_name
    let x: Double
    let y: Double
    init(x: Double, y: Double) {
        self.x = x
        self.y = y
    }
    // swiftlint:enable identifier_name
}

// MARK: - FOR PIXEL EDITOR

struct IncomingPixelActionMessage: ActionMessage, Codable {
    let type: String
    let action: Action
    let drawing: IncomingDrawingId
    let author: Author
    let pixels: [IncomingPixels]
}

struct IncomingPixels: Codable {
    // swiftlint:disable identifier_name
    let x: Double
    let y: Double
    // swiftlint:enable identifier_name
    let color: String
}

struct OutgoingPixelActionMessage: ActionMessage, Codable {
    let type: String
    let action: Action
    let drawing: OutgoingDrawing
    let pixels: [OutgoingPixels]

    init(actionId: Int, actionName: String, drawingId: String, pixels: [OutgoingPixels]) {
        self.type = OutgoingMessageConstants.pixelAction.rawValue
        self.action = Action(id: actionId, name: actionName)
        self.drawing = OutgoingDrawing(id: drawingId)
        self.pixels = pixels
    }
}

struct OutgoingPixels: Codable {
    // swiftlint:disable identifier_name
    let x: Double
    let y: Double
    let color: String
    init(x: Double, y: Double, color: String) {
        self.x = x
        self.y = y
        self.color = color
    }
    // swiftlint:enable identifier_name
}
