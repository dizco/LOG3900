//
//  Timestamp.swift
//  PolyPaintPro
//
//  Created by Kenny Nguyen on 2018-02-05.
//  Copyright © 2018 Les Pods c'est pour les lunchs. All rights reserved.
//

import Foundation

class Timestamp {
    let dateFormatter = DateFormatter()

    init() {
        self.dateFormatter.dateFormat = "HH:mm:ss"
    }

    func getTimeFromServer(timestamp: Double) -> String {
        let serverTime = Date(timeIntervalSince1970: (timestamp / 1000))
        return self.dateFormatter.string(from: serverTime)
    }
}
