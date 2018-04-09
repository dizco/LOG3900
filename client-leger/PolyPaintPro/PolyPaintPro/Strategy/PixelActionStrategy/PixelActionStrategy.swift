//
//  PixelActionStrategy.swift
//  PolyPaintPro
//
//  Created by Kenny Nguyen on 2018-04-09.
//  Copyright © 2018 Les Pods c'est pour les lunchs. All rights reserved.
//

import Foundation

protocol PixelActionStrategy {
    func applyReceived(viewController: PixelEditorViewController, incomingAction: IncomingPixelActionMessage)
}
