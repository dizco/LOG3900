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
    let socket = WebSocket(url: URL(string: "ws://localhost:5025/")!)
    weak var delegate: SocketManagerDelegate?

    private init() {
        socket.onConnect = {
            self.delegate?.connect()
        }

        socket.onDisconnect = { error in
            self.delegate?.disconnect(error: error)
        }

        socket.onData = { data in
            self.delegate?.managerDidReceive(data: data)
        }

        socket.onText = { text in
            let data = text.data(using: .utf16)!
            self.delegate?.managerDidReceive(data: data)
        }
        socket.connect()
    }

    func send(data: Data) {
        if !socket.isConnected {
            print ("Connection is not established.")
            return
        }
        socket.write(data: data)
    }

    func establishConnection() {
        socket.connect()
    }

    func closeConnection() {
        socket.disconnect()
    }
}
