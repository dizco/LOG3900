//
//  IncomingMessageConstants.swift
//  PolyPaintPro
//
//  Created by Kenny Nguyen on 2018-03-23.
//  Copyright © 2018 Les Pods c'est pour les lunchs. All rights reserved.
//

import Foundation

enum IncomingMessageConstants: String {
    case strokeAction = "server.editor.stroke.action"
    case chat = "server.chat.message"
}

enum OutgoingMessageConstants: String {
    case strokeAction = "client.editor.stroke.action"
    case chat = "client.chat.message"
}
