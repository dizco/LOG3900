//
//  AddPixelActionStrategy.swift
//  PolyPaintPro
//
//  Created by Kenny Nguyen on 2018-04-09.
//  Copyright © 2018 Les Pods c'est pour les lunchs. All rights reserved.
//

import Foundation
import UIKit

final class AddPixelActionStrategy: PixelActionStrategy {
    func applyReceived(viewController: PixelEditorViewController, incomingAction: IncomingPixelActionMessage) {
        self.drawReceived(viewController: viewController, incomingPixels: incomingAction.pixels)
    }

    private func drawReceived(viewController: PixelEditorViewController, incomingPixels: [IncomingPixels]) {
        //UIGraphicsBeginImageContextWithOptions(viewController.view.bounds.size, false, 0)
        UIGraphicsBeginImageContext(viewController.view.bounds.size)
        for pixel in incomingPixels {
            self.drawPixel(viewController: viewController, pixel: pixel)
        }

        let image = UIGraphicsGetImageFromCurrentImageContext()
        UIGraphicsEndImageContext()

        viewController.imageView.image = image
    }

    private func drawPixel(viewController: PixelEditorViewController, pixel: IncomingPixels) {
        viewController.imageView.image?.draw(in: viewController.view.bounds)

        let context = UIGraphicsGetCurrentContext()
        let point = CGPoint(x: pixel.x, y: pixel.y)

        context?.move(to: point)
        context?.addLine(to: point)

        context?.setLineCap(CGLineCap.round)
        let color = self.convertHexToUIColor(hex: pixel.color)
        context?.setStrokeColor(color!.cgColor)
        context?.setBlendMode(CGBlendMode.normal)
        context?.strokePath()
    }

    private func convertHexToUIColor(hex: String) -> UIColor? {
        // https://cocoacasts.com/from-hex-to-uicolor-and-back-in-swift
        // thank you mr skeltal
        var rgb: UInt32 = 0

        var red: CGFloat = 0.0
        var green: CGFloat = 0.0
        var blue: CGFloat = 0.0
        var alpha: CGFloat = 0.0

        var hexSanitized = hex.trimmingCharacters(in: .whitespacesAndNewlines)
        hexSanitized = hexSanitized.replacingOccurrences(of: "#", with: "")
        let length = hexSanitized.count

        guard Scanner(string: hexSanitized).scanHexInt32(&rgb) else { return nil }

        if length == 6 {
            red = CGFloat((rgb & 0xFF0000) >> 16) / 255.0
            green = CGFloat((rgb & 0x00FF00) >> 8) / 255.0
            blue = CGFloat(rgb & 0x0000FF) / 255.0

        } else if length == 8 {
            alpha = CGFloat((rgb & 0xFF000000) >> 24) / 255.0
            red = CGFloat((rgb & 0x00FF0000) >> 16) / 255.0
            green = CGFloat((rgb & 0x0000FF00) >> 8) / 255.0
            blue = CGFloat(rgb & 0x000000FF) / 255.0
        } else {
            return nil
        }

        return UIColor(red: red, green: green, blue: blue, alpha: alpha)
    }

    private func convertPixelToCGPoint(pixel: IncomingPixels) -> CGPoint {
        return CGPoint(x: pixel.x, y: pixel.y)
    }
}
