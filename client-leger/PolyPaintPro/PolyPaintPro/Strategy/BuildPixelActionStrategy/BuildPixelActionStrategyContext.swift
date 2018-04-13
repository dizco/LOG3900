//
//  BuildPixelActionStrategyContext.swift
//  PolyPaintPro
//
//  Created by Kenny Nguyen on 2018-04-09.
//  Copyright © 2018 Les Pods c'est pour les lunchs. All rights reserved.
//

import Foundation

final class BuildPixelActionStrategyContext {
    let outgoingActionMessage: OutgoingPixelActionMessage?

    init(viewController: PixelEditorViewController, actionId: Int, fromPixel: UIPixel, toPixel: UIPixel) {
        switch actionId {
        case PixelActionIdConstants.add.rawValue:
            self.outgoingActionMessage = AddBuildPixelActionStrategy().buildOutgoingAction(viewController: viewController, actionId: actionId, fromPixel: fromPixel, toPixel: toPixel)

        default:
            self.outgoingActionMessage = nil
        }
    }

    func getOutgoingMessage() -> OutgoingPixelActionMessage? {
        return self.outgoingActionMessage
    }
}
