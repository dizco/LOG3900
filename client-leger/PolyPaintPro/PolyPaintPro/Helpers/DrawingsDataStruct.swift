//
//  DrawingsDataStruct.swift
//  PolyPaintPro
//
//  Created by Fred Habsfan on 2018-03-23.
//  Copyright © 2018 Les Pods c'est pour les lunchs. All rights reserved.
//

import Foundation

struct JoinDrawingsDataStruct: Codable {
    let id: String
    let name: String
    let privacyStatus: Bool //true = protected, false = public
    let type: String
}

struct OpenLocalDrawingsDataStruct: Codable {
    let id: String
    let name: String
    let type: String
}
