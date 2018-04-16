//
//  AddBuildPixelActionStrategy.swift
//  PolyPaintPro
//
//  Created by Kenny Nguyen on 2018-04-09.
//  Copyright © 2018 Les Pods c'est pour les lunchs. All rights reserved.
//

import Foundation
import UIKit

final class AddBuildPixelActionStrategy: BuildPixelActionStrategy {
    func buildOutgoingAction(viewController: PixelEditorViewController, actionId: Int, pixels: [UIPixel]) -> OutgoingPixelActionMessage {
        var outgoingPixels: [OutgoingPixels] = []

        for pixelToBeConverted in pixels {
            let pixelToBeSent = self.convertCGPointToPixel(pixel: pixelToBeConverted)
            outgoingPixels.append(pixelToBeSent)
        }

        return OutgoingPixelActionMessage(actionId: actionId, actionName: PixelActionNameConstants.addActionName.rawValue,
                                          drawingId: viewController.drawing!.id, pixels: outgoingPixels)
    }

    private func convertUIColorToHex(color: SKStrokeColor, withAlpha: Bool = true) -> String? {
        // https://cocoacasts.com/from-hex-to-uicolor-and-back-in-swift
        let red = Float(color.red)
        let green = Float(color.green)
        let blue = Float(color.blue)
        let alpha = Float(color.alphaValue)

        // swiftlint:disable line_length
        if withAlpha {
            return String(format: "#%02lX%02lX%02lX%02lX", lroundf(alpha * 255), lroundf(red * 255), lroundf(green * 255), lroundf(blue * 255))
        } else {
            return String(format: "#%02lX%02lX%02lX", lroundf(red * 255), lroundf(green * 255), lroundf(blue * 255))
        }
        // swiftlint:enable line_length
    }

    private func convertCGPointToPixel(pixel: UIPixel) -> OutgoingPixels {
        let color = self.convertUIColorToHex(color: pixel.color)
        return OutgoingPixels(x: Double(pixel.point.x), y: Double(pixel.point.y), color: color!)
    }

    private func pixelsAreTheSame(pixel1: UIPixel, pixel2: UIPixel) -> Bool {
        if pixel1.point.x == pixel2.point.x && pixel1.point.y == pixel2.point.y {
            return true
        } else {
            return false
        }
    }
}
