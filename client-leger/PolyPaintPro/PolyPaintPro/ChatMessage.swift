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
}

struct OutgoingChatMessage: ChatMessage, Codable {
    var type: String
    var room: OutgoingRoom
    var message: String
    var author: Author

    init(message: String) {
        self.type = "client.chat.message"
        self.room = OutgoingRoom(id: "chat")
        self.message = message
        self.author = Author(id: -1, username: "", name: "", url: "", avatarUrl: "")
    }
}

struct IncomingChatMessage: ChatMessage, Codable {
    let type: String
    let room: IncomingRoom
    let message: String
    let author: Author
    // let timestamp: Int
    // server isn't sending us timestamp yet
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

    enum CodingKeys: String, CodingKey {
        case id
        case username
        case name
        case url
        case avatarUrl = "avatar_url"
    }
}
