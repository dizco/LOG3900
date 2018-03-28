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
    private class func buildUrl(endpoint: String) -> String {
        return Rest.Connection.HttpProtocol
            + ServerLookup.sharedInstance.address
            + Rest.Connection.DefaultPort
            + endpoint
    }

        }

    static func loginToServer(username: String, password: String) -> Promise<AuthServerResponse<EmptyData>> {
        let headers = ["Content-Type": "application/x-www-form-urlencoded"]
        let parameters: [String: String] = [
            "email": username,
            "password": password
        ]
        return Promise<AuthServerResponse> { fulfill, reject in
            Alamofire.request(self.buildUrl(endpoint: Rest.Routes.Login),
                              method: .post,
                              parameters: parameters,
                              encoding: URLEncoding.default,
                              headers: headers)
                .responseJSON { response in
                    do {
                        if let data = response.data,
                            let json = try JSONSerialization.jsonObject(with: data, options: []) as? [String: Any],
                            let authServerResponse = AuthServerResponse<EmptyData>(json: json) {
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

    static func registerToServer(username: String, password: String) -> Promise<AuthServerResponse<EmptyData>> {
        let headers = ["Content-Type": "application/x-www-form-urlencoded"]
        let parameters: [String: String] = [
            "email": username,
            "password": password
        ]
        return Promise<AuthServerResponse> { fulfill, reject in
            Alamofire.request(self.buildUrl(endpoint: Rest.Routes.Register),
                                method: .post,
                                parameters: parameters,
                                encoding: URLEncoding.default,
                                headers: headers)
                .responseJSON { response in
                    do {
                        if let data = response.data,
                            let json = try JSONSerialization.jsonObject(with: data, options: []) as? [String: Any],
                            let authServerResponse = AuthServerResponse<EmptyData>(json: json) {
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

    static func testServerConnection() -> Promise<AuthServerResponse<EmptyData>> {
        return Promise<AuthServerResponse> { fulfill, _ in
            Alamofire.request(self.buildUrl(endpoint: Rest.Routes.Ping))
                .responseJSON { (response) -> Void in
                    fulfill(AuthServerResponse(success: self.isValidResponse(response: response)))
                }
            }
    }

    private static func isValidResponse(response: DataResponse<Any>) -> Bool {
        if let resp = response.response {
            return resp.statusCode >= 200 && resp.statusCode <= 299
        }
        return false
    }

    public struct EmptyData: Codable {}

    public struct AuthServerResponse<T: Codable> {
        let data: T?
        let success: Bool
        let error: String

        init(success: Bool, error: String = "") {
            self.data = nil
            self.success = success
            self.error = error
        }

        //Init via a raw json server response
        init?(json: [String: Any]) {
            guard let status = json["status"] as? String
                else {
                    return nil
            }

            self.data = nil
            self.success = status == "success"
            self.error = json["error"] as? String ?? ""
        }
    }
}
