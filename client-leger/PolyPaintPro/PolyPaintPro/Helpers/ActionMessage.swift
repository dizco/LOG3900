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

struct Action: Codable {
    let id: Int
    let name: String
    init(id: Int, name: String) {
        self.id = id
        self.name = name
    }
}

// MARK: - IncomingActionMessage
struct IncomingActionMessage: ActionMessage, Codable {
    let type: String
    let action: Action
    let drawing: IncomingDrawing
    let author: Author
    let delta: Delta
    let timestamp: Double
}

struct IncomingDrawing: Codable {
    let id: String
    //let name: String
    //let protection: IncomingProtection
    //let owner: IncomingOwner
}

// Will delete here, once I have confirmation that this is not used anymore.

/*struct IncomingProtection: Codable {
    let active: Bool
}

struct IncomingOwner: Codable {
    let username: String
    let url: String
    let avatarUrl: String

    enum CodingKeys: String, CodingKey {
        case username
        case url
        case avatarUrl = "avatar_url"
    }
}*/

struct Delta: Codable {
    let add: [IncomingAdd]
    let remove: [String]
}

struct IncomingAdd: Codable {
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

// MARK: - OutgoingActionMessage
struct OutgoingActionMessage: ActionMessage, Codable {
    let type: String
    let action: Action
    let drawing: OutgoingDrawing
    let delta: OutgoingDelta

    init(actionId: Int, actionName: String, delta: OutgoingDelta) {
        self.type = OutgoingMessageConstants.strokeAction.rawValue
        self.action = Action(id: actionId, name: actionName)
        self.drawing = OutgoingDrawing(id: "5ab91791ae6a83a7d8e4fa60") // TO-DO : fetch the good id from AccountManager after subscription
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
