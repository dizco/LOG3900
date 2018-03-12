//
//  Login.swift
//  PolyPaintPro
//
//  Created by Kenny Nguyen on 2018-02-08.
//  Copyright © 2018 Les Pods c'est pour les lunchs. All rights reserved.
//

import Foundation
import Alamofire
import PromiseKit

class RestManager {
    var username: String
    var password: String

    init(username: String, password: String) {
        self.username = username.lowercased()
        self.password = password
    }

    func buildUrl(endpoint: String) -> String {
        return Rest.Connection.HttpProtocol
            + ServerLookup.sharedInstance.address
            + Rest.Connection.DefaultPort
            + endpoint
    }

    func loginToServer() -> Promise<AuthServerResponse> {
        if ServerLookup.sharedInstance.address.isEmpty {
            return Promise(value: AuthServerResponse(success: false, error: Rest.Auth.NoServerAddress))
        } else {
            return Promise<AuthServerResponse> { fulfill, reject in
                let headers = ["Content-Type": "application/x-www-form-urlencoded"]
                let parameters: [String: String] = [
                    "email": self.username,
                    "password": self.password
                ]
                Alamofire.request(self.buildUrl(endpoint: Rest.Auth.Routes.Login),
                                  method: .post,
                                  parameters: parameters,
                                  encoding: URLEncoding.default,
                                  headers: headers)
                    .responseJSON { response in
                        do {
                            if let data = response.data,
                                let json = try JSONSerialization.jsonObject(with: data, options: []) as? [String: Any],
                                let authServerResponse = AuthServerResponse(json: json) {
                                if authServerResponse.success {
                                    AccountManager.sharedInstance.saveCookies(response: response)
                                }
                                fulfill(authServerResponse)
                            } else {
                                fulfill(AuthServerResponse(success: false))
                            }
                        } catch let error {
                            reject(error)
                        }
                }
            }
        }
    }

    func registerToServer() -> Promise<AuthServerResponse> {
        if ServerLookup.sharedInstance.address.isEmpty {
            return Promise(value: AuthServerResponse(success: false, error: Rest.Auth.NoServerAddress))
        } else {
            return Promise<AuthServerResponse> { fulfill, reject in
                let headers = ["Content-Type": "application/x-www-form-urlencoded"]
                let parameters: [String: String] = [
                    "email": self.username,
                    "password": self.password
                ]
                Alamofire.request(self.buildUrl(endpoint: Rest.Auth.Routes.Register),
                                  method: .post,
                                  parameters: parameters,
                                  encoding: URLEncoding.default,
                                  headers: headers)
                    .responseJSON { response in
                        do {
                            if let data = response.data,
                                let json = try JSONSerialization.jsonObject(with: data, options: []) as? [String: Any],
                                let authServerResponse = AuthServerResponse(json: json) {
                                fulfill(authServerResponse)
                                return
                            } else {
                                fulfill(AuthServerResponse(success: false))
                            }
                        } catch let error {
                            reject(error)
                        }
                }
            }
        }
    }

    func testServerConnection() -> Promise<AuthServerResponse> {
        if ServerLookup.sharedInstance.address.isEmpty {
            return Promise(value: AuthServerResponse(success: false, error: Rest.Auth.NoServerAddress))
        } else {
            return Promise<AuthServerResponse> { fulfill, _ in
                Alamofire.request(self.buildUrl(endpoint: Rest.Auth.Routes.Register),
                                  method: .get,
                                  encoding: JSONEncoding.default)
                    .responseJSON { (response) -> Void in
                        if self.isValidResponse(response: response) {
                            fulfill(AuthServerResponse(success: true))
                        } else {
                            fulfill(AuthServerResponse(success: false))
                        }
                }
            }
        }
    }

    private func isValidResponse(response: DataResponse<Any>) -> Bool {
        if let resp = response.response {
            return resp.statusCode >= 200 && resp.statusCode <= 299
        }
        return false
    }

    public struct AuthServerResponse: Codable {
        let success: Bool
        let error: String

        init(success: Bool, error: String = "") {
            self.success = success
            self.error = error
        }

        //Init via a raw json server response
        init?(json: [String: Any]) {
            guard let status = json["status"] as? String
                else {
                    return nil
            }

            self.success = status == "success"
            self.error = json["error"] as? String ?? ""
        }
    }
}
