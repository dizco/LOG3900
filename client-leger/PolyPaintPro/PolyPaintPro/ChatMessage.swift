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
    var message: String { get }
    var author: Author { get }
    var timestamp: Int { get }
}

struct OutgoingChatMessage: ChatMessage, Codable {
    var type: String
    var room: OutgoingRoom
    var message: String
    var author: Author
    var timestamp: Int

    init(message: String) {
        self.type = "client.chat.message"
        self.room = OutgoingRoom(id: "chat")
        self.message = message
        self.author = Author(id: -1, username: "", name: "", url: "", avatarUrl: "")
        self.timestamp = -1
    }
}

struct IncomingChatMessage: ChatMessage, Codable {
    let type: String
    let room: IncomingRoom
    let message: String
    let author: Author
    let timestamp: Int
}

struct OutgoingRoom: Codable {
    let id: String
}

struct IncomingRoom: Codable {
    let id: String
    let name: String
}

struct Author: Codable {
    let id: Int
    let username: String
    let name: String
    let url: String
    let avatarUrl: String
}

/*
enum MessageSource {
    case client, server
}

enum MessageFactory {
    static func message(for source: MessageSource, fromServer: [String: Any]) -> ChatMessage? {
        switch source {
        case .client:
            return IncomingChatMessage()
        case .server:
            return OutgoingChatMessage()
        }
    }
}*/
