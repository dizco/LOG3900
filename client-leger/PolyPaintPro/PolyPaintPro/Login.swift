//
//  Login.swift
//  PolyPaintPro
//
//  Created by Kenny Nguyen on 2018-02-08.
//  Copyright © 2018 Les Pods c'est pour les lunchs. All rights reserved.
//

import Foundation
import Alamofire

class Login {
    var username = AccountManager.sharedInstance.username
    var password: String
    var error: String?

    init(password: String) {
        self.password = password
    }
    
    func connectToServer() -> Bool {
        if ServerLookup.sharedInstance.address.isEmpty {
            self.error = "No server address found."
            return false
        } else {
            let headers = ["Content-Type": "application/x-www-form-urlencoded"]
            let parameters: [String: String] = [
                "email": self.username!,
                "password": self.password
            ]

            var isConnected = false
            Alamofire.request("http://" + ServerLookup.sharedInstance.address + ":5025/login",
                              method: .post,
                              parameters: parameters,
                              encoding: URLEncoding.default,
                              headers: headers).responseJSON { response in
                                do {
                                    let decoder = JSONDecoder()
                                    let serverResponse = try decoder.decode(ServerResponse.self, from: response.data!)
                                    print(serverResponse.status)
                                    if serverResponse.status == "success" {
                                        isConnected = true
                                        self.password = ""
                                    } else {
                                        self.error = "Your email and/or your password isn't valid."
                                    }
                                } catch let error {
                                    print(error)
                                }
            }
            return isConnected
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
