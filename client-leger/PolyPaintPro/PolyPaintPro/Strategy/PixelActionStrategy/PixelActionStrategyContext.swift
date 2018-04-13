//
//  PixelActionStrategyContext.swift
//  PolyPaintPro
//
//  Created by Kenny Nguyen on 2018-04-09.
//  Copyright © 2018 Les Pods c'est pour les lunchs. All rights reserved.
//

import Foundation

final class PixelActionStrategyContext {
    init(viewController: PixelEditorViewController, incomingAction: IncomingPixelActionMessage) {
        let actionName = incomingAction.action.name

        switch actionName {
        case PixelActionNameConstants.addActionName.rawValue:
            AddPixelActionStrategy().applyReceived(viewController: viewController, incomingAction: incomingAction)
        default:
            print("Unknown IncomingPixelAction.")
        }
    }
}
