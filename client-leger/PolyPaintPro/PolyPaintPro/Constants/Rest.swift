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

    struct Auth {
        struct Routes {
            static let Login = "/login"
            static let Register = "/register"
        }
        static let NoServerAddress = "No server address found."
    }

    struct Ping {
        struct Routes {
            static let Ping = "/ping"
        }
    }
}
