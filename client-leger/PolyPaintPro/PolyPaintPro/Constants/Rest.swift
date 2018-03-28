//
//  RestConstants.swift
//  PolyPaintPro
//
//  Created by Gabriel Bourgault on 2018-03-12.
//  Copyright © 2018 Les Pods c'est pour les lunchs. All rights reserved.
//

import Foundation

struct Rest {
    struct Connection {
        static let DefaultPort = ":5025"
        static let HttpProtocol = "http://"
    }

    struct Routes {
        // MARK: - Auth routes
        static let Login = "/login"
        static let Register = "/register"

        // MARK: - Drawings routes
        static let Drawings = "/drawings"

        // MARK: - Ping routes
        static let Ping = "/ping"
    }
    
    struct Auth {
        static let NoServerAddress = "No server address found."
    }
}
