//
//  ChatMessage.swift
//  PolyPaintPro
//
//  Created by Kenny Nguyen on 2018-01-30.
//  Copyright © 2018 Les Pods c'est pour les lunchs. All rights reserved.
//

import Foundation

protocol ChatMessage {
    var type: String { get }
    var room: [String: Any] { get }
    var message: String { get }
    var author: [String: Any] { get }
    var timestamp: Int { get }

    func createJSON(withMsg: String) -> [String: Any]
}

class OutgoingChatMessage: ChatMessage {
    var type: String
    var room: [String: Any]
    var message: String
    var author: [String: Any]
    var timestamp: Int

    init() {
        self.type = "client.chat.message"
        self.room = ["id": "chat"]
        self.message = ""
        self.author = ["": ""]
        self.timestamp = -1
    }

    func createJSON(withMsg: String) -> [String: Any] {
        return [ "type": type, "room": room, "message": withMsg ]
    }
}

class IncomingChatMessage: ChatMessage {
    var type: String
    var room: [String: Any]
    var message: String
    var author: [String: Any]
    var timestamp: Int

    init(json: [String: Any]) {
        // TO-DO: Try doing it without force casting.

        // swiftlint:disable force_cast
        self.type = json["type"] as! String
        self.room = json["room"] as! [String: Any]
        self.message = json["message"] as! String
        self.author = json["author"] as! [String: Any]
        self.timestamp = json["timestamp"] as! Int
        // swiftlint:enable force_cast
    }

    func createJSON(withMsg: String) -> [String: Any] {
        return [ "": "" ]
    }
}

enum MessageSource {
    case client, server
}

enum MessageFactory {
    static func message(for source: MessageSource, fromServer: [String: Any]) -> ChatMessage? {
        switch source {
        case .client:
            return IncomingChatMessage(json: fromServer)
        case .server:
            return OutgoingChatMessage()
        }
    }
}
