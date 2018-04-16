//
//  BuildPixelActionStrategy.swift
//  PolyPaintPro
//
//  Created by Kenny Nguyen on 2018-04-09.
//  Copyright © 2018 Les Pods c'est pour les lunchs. All rights reserved.
//

import Foundation

protocol BuildPixelActionStrategy {
    func buildOutgoingAction(viewController: PixelEditorViewController, actionId: Int, pixels: [UIPixel]) -> OutgoingPixelActionMessage
}
