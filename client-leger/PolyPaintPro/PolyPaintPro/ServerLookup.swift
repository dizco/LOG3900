//
//  ServerLookup.swift
//  PolyPaintPro
//
//  Created by Kenny Nguyen on 2018-02-05.
//  Copyright © 2018 Les Pods c'est pour les lunchs. All rights reserved.
//

import Foundation

class ServerLookup {
    static let sharedInstance = ServerLookup()

    var address: String
    var error: String

    private init() {
        self.address = "localhost"
        self.error = ""
    }

    func saveServerAddress(withIPAddress: String) -> Bool {
        if verifyIPAddress(address: withIPAddress) {
            self.address = withIPAddress
            return true
        }
        return false
    }

    private func verifyIPAddress(address: String) -> Bool {
        let validRegex = "^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$"

        let match = address.range(of: validRegex, options: .regularExpression)

        if address.isEmpty {
            self.error = "Une adresse IP est requise."
            return false
        } else if match != nil {
            self.error = ""
            return true
        } else {
            self.error = "L'adresse IP est invalide."
            return false
        }
    }
}
