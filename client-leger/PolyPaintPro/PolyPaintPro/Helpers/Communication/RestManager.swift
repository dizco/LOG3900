import Foundation
import Alamofire
import PromiseKit

class RestManager {
    private class func buildUrl(endpoint: String,
                                parameters: [String: String]? = nil) -> String {
        var url = Rest.Connection.HttpProtocol
            + ServerLookup.sharedInstance.address
            + Rest.Connection.DefaultPort
            + endpoint
        if let parameters = parameters {
            url += "?"
            for (parameterName, parameterValue) in parameters {
                url += String(parameterName) + "=" + String(parameterValue) + "&"
            }
            url.remove(at: url.index(before: url.endIndex)) //Remove last character
        }
        return url
    }

    static func loginToServer(username: String, password: String) -> Promise<AuthServerResponse<IdResponse>> {
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
                            var authServerResponse = AuthServerResponse<IdResponse>(json: json) {
                            	if authServerResponse.success {
                                    authServerResponse.data = IdResponse(id: json["objectId"] as! String)
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
            after(seconds: 3).then {
                fulfill(AuthServerResponse(success: false)) //Took too long, reject
            }
            Alamofire.request(self.buildUrl(endpoint: Rest.Routes.Ping))
                .responseJSON { (response) -> Void in
                    fulfill(AuthServerResponse(success: self.isValidResponse(response: response)))
            }
        }
    }

    static func getDrawingsListPage(page: Int = 1, userId: String? = nil,
                                    visibility: String? = nil) -> Promise<AuthServerResponse<PaginatedDrawingsResponse>> {

        var parameters: [String: String] = ["page": String(page)]
        if let userId = userId {
            parameters["owner"] = userId
        }
        if let visibility = visibility {
            parameters["visibility"] = visibility
        }
        return Promise<AuthServerResponse> { fulfill, reject in
            Alamofire.request(self.buildUrl(endpoint: Rest.Routes.Drawings, parameters: parameters))
                .responseJSON { response in
                    do {
                        if let data = response.data {
                            let paginatedResponse = try JSONDecoder().decode(PaginatedDrawingsResponse.self, from: data)
                            let authServerResponse =
                                AuthServerResponse<PaginatedDrawingsResponse>(data: paginatedResponse)
                            fulfill(authServerResponse!)
                        } else {
                            fulfill(AuthServerResponse(success: false))
                        }
                    } catch let error {
                        reject(error)
                    }
            }
        }
    }

    static func getDrawingThumbnail(drawingId: String) -> Promise<AuthServerResponse<String>> {
        return Promise<AuthServerResponse> { fulfill, reject in
            Alamofire.request(self.buildUrl(endpoint: Rest.Routes.Drawings + "/" + drawingId + Rest.Routes.Thumbnail))
                .responseJSON { response in
                    do {
                        if let data = response.data,
                            let json = try JSONSerialization.jsonObject(with: data, options: []) as? [String: Any],
                            let authServerResponse = AuthServerResponse<String>(data: json["thumbnail"] as? String) {
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

    static func patchDrawing(drawingId: String,
                             visibility: String) -> Promise<AuthServerResponse<String>> {
        let headers = ["Content-Type": "application/x-www-form-urlencoded"]
        let parameters: [String: String] = [
            "visibility": visibility
        ]
        return Promise<AuthServerResponse> { fulfill, reject in
            Alamofire.request(self.buildUrl(endpoint: Rest.Routes.Drawings + "/" + drawingId),
                              method: .patch,
                              parameters: parameters,
                              encoding: URLEncoding.default,
                              headers: headers)
                .responseJSON { response in
                    do {
                        if let data = response.data,
                            let json = try JSONSerialization.jsonObject(with: data, options: []) as? [String: Any],
                            let authServerResponse = AuthServerResponse<String>(json: json) {
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

    static func postDrawing(name: String, mode: String, visibility: String, protectionActive: Bool,
                            protectionPassword: String = "") -> Promise<AuthServerResponse<IdResponse>> {
        let headers = ["Content-Type": "application/x-www-form-urlencoded"]
        let parameters: [String: String] = [
            "name": name,
            "mode": mode,
            "visibility": visibility
        ]
        return Promise<AuthServerResponse> { fulfill, reject in
            Alamofire.request(self.buildUrl(endpoint: Rest.Routes.Drawings),
                              method: .post,
                              parameters: parameters,
                              encoding: URLEncoding.default,
                              headers: headers)
                .responseJSON { response in
                    do {
                        if let data = response.data,
                            let json = try JSONSerialization.jsonObject(with: data, options: []) as? [String: Any],
                            var authServerResponse = AuthServerResponse<IdResponse>(json: json) {
                            if authServerResponse.success {
                                authServerResponse.data = IdResponse(id: json["objectId"] as! String)
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

    static func getDrawing(id: String,
                           protectionPassword: String = "") -> Promise<AuthServerResponse<IncomingDrawing>> {
        var headers: [String: String] = [:]
        if !protectionPassword.isEmpty {
            headers["protection-password"] = protectionPassword
        }
        return Promise<AuthServerResponse> { fulfill, reject in
            Alamofire.request(self.buildUrl(endpoint: Rest.Routes.Drawings + "/" + id),
                              method: .get,
                              encoding: URLEncoding.default,
                              headers: headers)
                .responseJSON { response in
                    do {
                        if let data = response.data {
                            let paginatedResponse = try JSONDecoder().decode(IncomingDrawing.self, from: data)
                            let authServerResponse =
                                AuthServerResponse<IncomingDrawing>(data: paginatedResponse)
                            fulfill(authServerResponse!)
                        } else {
                            fulfill(AuthServerResponse(success: false))
                        }
                    } catch let error {
                        reject(error)
                    }
            }
        }
    }

    private static func isValidResponse(response: DataResponse<Any>) -> Bool {
        if let resp = response.response {
            return resp.statusCode >= 200 && resp.statusCode <= 299
        }
        return false
    }

    public struct IdResponse: Codable {
        let id: String
    }

    public struct EmptyData: Codable {}

    public struct AuthServerResponse<T: Codable> {
        var data: T?
        let success: Bool
        let error: String

        init(success: Bool, error: String = "") {
            self.data = nil
            self.success = success
            self.error = error
        }

        init?(data: T?) {
            self.data = data
            self.success = true
            self.error = ""
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
