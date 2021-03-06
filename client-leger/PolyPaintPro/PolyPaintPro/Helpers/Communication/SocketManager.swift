//
//  SocketManager.swift
//  PolyPaintPro
//
//  Created by Kenny Nguyen on 2018-01-30.
//  Copyright © 2018 Les Pods c'est pour les lunchs. All rights reserved.
//

import Foundation
import Starscream

protocol SocketManagerDelegate: class {
    func connect()
    func disconnect(error: Error?)
}

protocol ChatSocketManagerDelegate: SocketManagerDelegate {
    func managerDidReceiveChat(data: Data)
}

protocol ActionSocketManagerDelegate: SocketManagerDelegate {
    func managerDidReceiveAction(data: Data)
}

private struct SocketMessageType: Codable {
    let type: String
}

class SocketManager {
    static let sharedInstance = SocketManager()
    private var socket: WebSocket?

    weak var chatDelegate: ChatSocketManagerDelegate?
    weak var actionDelegate: ActionSocketManagerDelegate?

    func send(data: Data) {
        if !socket!.isConnected {
            print ("Connection is not established.")
            return
        }
        socket!.write(data: data)
    }

    func getConnectionStatus() -> Bool {
        if self.socket == nil {
            return false
        } else {
            return self.socket!.isConnected
        }
    }

    func establishConnection(ipAddress: String = "localhost") {
        let serverUrl = URL(string: "ws://" + ipAddress + ":5025/")!

        // Retrieve the COOKIES!
        let jar = HTTPCookieStorage.shared
        let cookies = jar.cookies
        jar.setCookies(cookies!, for: serverUrl, mainDocumentURL: serverUrl)
        print("\(cookies!)")
        let cookieInfo = "connect.sid=" + cookies![0].value

        // Send the COOKIES!
        var request = URLRequest(url: serverUrl)
        request.setValue(cookieInfo, forHTTPHeaderField: "cookie")
        self.socket = WebSocket(request: request)

        self.socket!.onConnect = {
            self.chatDelegate?.connect()
            self.actionDelegate?.connect()
        }

        self.socket!.onDisconnect = { error in
            self.chatDelegate?.disconnect(error: error)
            self.actionDelegate?.disconnect(error: error)
        }

        self.socket!.onText = { text in
            let data = text.data(using: .utf16)!
            let decoder = JSONDecoder()
            do {
                let socketMessageType = try decoder.decode(SocketMessageType.self, from: data)
                print(socketMessageType.type)
                if socketMessageType.type == IncomingMessageConstants.strokeAction.rawValue {
                    self.actionDelegate?.managerDidReceiveAction(data: data)
                } else if socketMessageType.type == IncomingMessageConstants.chat.rawValue {
                    self.chatDelegate?.managerDidReceiveChat(data: data)
                } else if socketMessageType.type == IncomingMessageConstants.pixelAction.rawValue {
                    self.actionDelegate?.managerDidReceiveAction(data: data)
                }
            } catch let error {
                print(error)
            }
        }
        socket!.connect()
    }

    func closeConnection() {
        socket!.disconnect()
    }
}
