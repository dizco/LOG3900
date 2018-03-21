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
    //let remove: String // this is currently received as null...
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
    var type: String
    var action: Action
    var drawing: OutgoingDrawing

    init() {
        self.type = "client.editor.action"
        self.action = Action(id: 1, name: "watsgoingon") // TO-DO : send the good action
        self.drawing = OutgoingDrawing(id: "errmagawd") // TO-DO : fetch the good id
    }
}

struct OutgoingDrawing: Codable {
    let id: String
    init(id: String) {
        self.id = id
    }
}
