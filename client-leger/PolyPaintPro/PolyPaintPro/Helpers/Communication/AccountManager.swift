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
    static let passwordMinLength = 8
    var user: UserIdentifiers?

    func isMyself(id: String) -> Bool {
        return id == self.user!.id
    }

    func saveUser(userId: String, username: String) -> AccountManagerResponse {
        if username.isEmpty {
            return AccountManagerResponse(success: false, error: "Un nom d'usager est requis.")
        } else {
            self.user = UserIdentifiers(id: userId, username: username.lowercased())
            return AccountManagerResponse(success: true)
        }
    }

    static func validateUsername(username: String) -> AccountManagerResponse {
        let validRegex = "[A-Z0-9a-z._%+-]+@[A-Za-z0-9.-]+\\.[A-Za-z]{2,64}"

        let match = username.range(of: validRegex, options: .regularExpression)

        if username.isEmpty {
            return AccountManagerResponse(success: false, error: "Un courriel est requis.")
        } else if match == nil {
            return AccountManagerResponse(success: false, error: "Le courriel est invalide.")
        }
        return AccountManagerResponse(success: true)
    }

    static func validatePassword(password: String) -> AccountManagerResponse {
        if password.isEmpty {
            return AccountManagerResponse(success: false, error: "Un mot de passe est requis.")
        } else if password.count < AccountManager.passwordMinLength {
            return AccountManagerResponse(success: false,
                                          error: "Un mot de passe doit avoir un minimum de 8 caractères.")
        }
        return AccountManagerResponse(success: true)
    }

    static func validateRegister(username: String, password: String) -> AccountManagerResponse {
        let usernameValidation = self.validateUsername(username: username)
        let passwordValidation = self.validatePassword(password: password)
        var errorMessage = ""
        if !usernameValidation.success {
            errorMessage += usernameValidation.error + "\n"
        } else if !passwordValidation.success {
            errorMessage += passwordValidation.error
        }
        return AccountManagerResponse(success: usernameValidation.success && passwordValidation.success,
                                      error: errorMessage)
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

    public struct UserIdentifiers: Codable {
        let id: String
        let username: String

        init(id: String, username: String) {
            self.id = id
            self.username = username
        }
    }

    public struct AccountManagerResponse {
        let success: Bool
        let error: String

        init(success: Bool, error: String = "") {
            self.success = success
            self.error = error
        }
    }
}
