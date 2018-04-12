//
//  DrawingsDataStruct.swift
//  PolyPaintPro
//
//  Created by Fred Habsfan on 2018-03-23.
//  Copyright © 2018 Les Pods c'est pour les lunchs. All rights reserved.
//

import Foundation

struct IncomingUsers: Codable {
    let active: Int
    let limit: Int
}

struct IncomingOwner: Codable {
    let id: String
    let username: String

    enum CodingKeys: String, CodingKey {
        case id = "_id"
        case username
    }
}

struct OnlineDrawingModel: Codable {
    let id: String
    let name: String
    let mode: String
    let owner: IncomingOwner
    let protection: IncomingProtection
    var visibility: String
    let users: IncomingUsers

    enum CodingKeys: String, CodingKey {
        case id = "_id"
        case name
        case mode
        case owner
        case protection
        case visibility
        case users
    }
}

struct PaginatedDrawingsResponse: Codable {
    let docs: [OnlineDrawingModel]
    let total: Int
    let limit: Int
    let page: String
    let pages: Int
}

struct LocalDrawingModel: Codable {
    let id: String
    let name: String
    let type: String
}
