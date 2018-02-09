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
    func managerDidReceive(data: Data)
}

class SocketManager {
    static let sharedInstance = SocketManager()
    private var socket: WebSocket?
    weak var delegate: SocketManagerDelegate?

    func send(data: Data) {
        if !socket!.isConnected {
            print ("Connection is not established.")
            return
        }
        socket!.write(data: data)
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
            self.delegate?.connect()
        }

        self.socket!.onDisconnect = { error in
            self.delegate?.disconnect(error: error)
        }

        self.socket!.onData = { data in
            self.delegate?.managerDidReceive(data: data)
        }

        self.socket!.onText = { text in
            let data = text.data(using: .utf16)!
            self.delegate?.managerDidReceive(data: data)
        }
        socket!.connect()
    }

    func closeConnection() {
        socket!.disconnect()
    }
}
