//
//  AccountManager.swift
//  PolyPaintPro
//
//  Created by Kenny Nguyen on 2018-02-08.
//  Copyright © 2018 Les Pods c'est pour les lunchs. All rights reserved.
//

import Foundation
import Alamofire

class AccountManager {
    static let sharedInstance = AccountManager()
    var username: String?
    var usernameError: String?
    var passwordError: String?
    var registerError: String?
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
        } else if password.count < self.passwordMinLength {
            self.passwordError = "Un mot de passe doit avoir un minimum de 8 caractères."
            return false
        } else {
            self.passwordError = ""
            return true
        }
    }

    func validateRegister(username: String, password: String) -> Bool {
        if !validateUsername(username: username) {
            self.registerError = self.usernameError
            return false
        } else if !validatePassword(password: password) {
            self.registerError = self.passwordError
            return false
        } else {
            self.registerError = ""
            return true
        }
    }

    func saveCookies(response: DataResponse<Any>) {
        let jar = HTTPCookieStorage.shared
        if let headerFields = response.response? // If impossible to cast, will be nil
            .allHeaderFields as? [String: String] {
            let url = response.response?.url
            let cookies = HTTPCookie.cookies(withResponseHeaderFields: headerFields, for: url!)
            jar.setCookies(cookies, for: url, mainDocumentURL: url)
        }
    }

    func loadCookies() {
        guard let cookieArray = UserDefaults.standard.array(forKey: "savedCookies") as? [[HTTPCookiePropertyKey: Any]]
            else { return }
        for cookieProperties in cookieArray {
            if let cookie = HTTPCookie(properties: cookieProperties) {
                HTTPCookieStorage.shared.setCookie(cookie)
            }
        }
    }
}
