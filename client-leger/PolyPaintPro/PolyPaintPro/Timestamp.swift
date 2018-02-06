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

    func getCurrentTime() -> String {
        let currentTime = Date()
        
        return self.dateFormatter.string(from: currentTime)
    }

    func getTimeFromServer() {
        // TO-DO: Convert the timestamp given by the server
    }
}
