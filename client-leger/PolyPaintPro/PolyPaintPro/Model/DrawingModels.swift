//
//  DrawingsDataStruct.swift
//  PolyPaintPro
//
//  Created by Fred Habsfan on 2018-03-23.
//  Copyright © 2018 Les Pods c'est pour les lunchs. All rights reserved.
//

import Foundation

struct OnlineDrawingModel: Codable {
    let id: String
    let name: String
    let protection: IncomingProtection
    let type: String
}

struct LocalDrawingModel: Codable {
    let id: String
    let name: String
    let type: String
}
