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

class Login {
    var username: String
    var password: String
    var error: String

    init(username: String, password: String) {
        self.username = username.lowercased()
        self.password = password
        self.error = ""
    }
    
    func connectToServer() -> Promise<Bool> {
        if ServerLookup.sharedInstance.address.isEmpty {
            error = "No server address found."
            return Promise(value: false)
        } else {
            return Promise<Bool> { fulfill, reject in
                let headers = ["Content-Type": "application/x-www-form-urlencoded"]
                let parameters: [String: String] = [
                    "email": self.username,
                    "password": self.password
                ]
                Alamofire.request("http://" + ServerLookup.sharedInstance.address + ":5025/login",
                                  method: .post,
                                  parameters: parameters,
                                  encoding: URLEncoding.default,
                                  headers: headers).responseJSON { response in
                                    let decoder = JSONDecoder()
                                    do {
                                        let serverResponse = try decoder.decode(ServerResponse.self, from: response.data!)
                                        if serverResponse.status == "success" {
                                            fulfill(true)
                                            //AccountManager.sharedInstance.saveCookies(response: response)
                                            return
                                        } else {
                                            fulfill(false)
                                        }
                                    } catch let error {
                                        reject(error)
                                    }
                }
            }
        }
    }

    func testServerConnection() {
        if ServerLookup.sharedInstance.address.isEmpty {
            error = "No server address found."
        } else {
            Alamofire.request(URL(string: "http://" + ServerLookup.sharedInstance.address + ":5025/ping")!,
                              method: .get,
                              encoding: JSONEncoding.default).responseJSON { (response) -> Void in
                                if response.response?.statusCode == 200 {
                                    print("No problem.")
                                } else {
                                    print("Oh boy, did the server eat a Tide Pod again?!")
                                }
            }
        }
    }

    private struct ServerResponse: Codable {
        let status: String
    }
}
