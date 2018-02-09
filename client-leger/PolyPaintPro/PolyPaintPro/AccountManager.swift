//
//  AccountManager.swift
//  PolyPaintPro
//
//  Created by Kenny Nguyen on 2018-02-08.
//  Copyright © 2018 Les Pods c'est pour les lunchs. All rights reserved.
//

import Foundation

class AccountManager {
    static let sharedInstance = AccountManager()
    //var username: String?
    var username: String?
    var usernameError: String?
    var passwordError: String?
    let passwordMinLength = 8

    func saveUsername(username: String) -> Bool {
        if username.isEmpty {
            self.usernameError = "Un nom d'usager est requis."
            return false
        } else {
            self.username = username.lowercased()
            return true
        }
    }
    
    func validateUsername(username: String) -> Bool {
        let validRegex = "[A-Z0-9a-z._%+-]+@[A-Za-z0-9.-]+\\.[A-Za-z]{2,64}"

        let match = username.range(of: validRegex, options: .regularExpression)

        if username.isEmpty {
            self.usernameError = "Un courriel est requis."
            return false
        } else if match != nil {
            self.usernameError = ""
            return self.saveUsername(username: username)
        } else {
            self.usernameError = "Le courriel est invalide."
            return false
        }
    }

    func validatePassword(password: String) -> Bool {
        if password.isEmpty {
            self.passwordError = "Un mot de passe est requis."
            return false
        } else {
            self.passwordError = ""
            return true
        }
    }
}
