//
//  IncomingMessageConstants.swift
//  PolyPaintPro
//
//  Created by Kenny Nguyen on 2018-03-23.
//  Copyright © 2018 Les Pods c'est pour les lunchs. All rights reserved.
//

import Foundation

enum IncomingMessageConstants: String {
    // MARK: - Incoming Stroke constants
    case strokeAction = "server.editor.stroke.action"

    // MARK: - Incoming Pixel constants
    case pixelAction = "server.editor.pixel.action"

    // MARK: - Incoming Chat constants
    case chat = "server.chat.message"
}

enum OutgoingMessageConstants: String {
    // MARK: - Outgoing Stroke constants
    case strokeAction = "client.editor.stroke.action"

    // MARK: - Outgoing Pixel constants
    case pixelAction = "client.editor.pixel.action"

    // MARK: - Outgoing Chat constants
    case chat = "client.chat.message"
}
