import UIKit
import Starscream
import SpriteKit

enum PixelEditingMode {
    case ink, select, eraseByPoint
}

class PixelEditorViewController: EditorViewController, PixelToolsViewDelegate {
    internal var lastPoint = CGPoint.zero //last drawn point on the canvas
    internal var red: CGFloat = 0.0 //RGB, stores the currend rgb value from the selector
    internal var green: CGFloat = 0.0
    internal var blue: CGFloat = 0.0
    internal var brushWidth: CGFloat = 10.0 //brush stroke and opacity
    internal var opacity: CGFloat = 1.0
    internal var swiped = false //if the brush stroke is continuous
    internal var currentEditingMode = PixelEditingMode.ink // will be used to switch editing modes

    override func didReceiveMemoryWarning() {
        super.didReceiveMemoryWarning()
    }

    override func viewDidLoad() {
        super.viewDidLoad()
        toolsView.pixelDeletage = self
    }

    override func viewDidAppear(_ animated: Bool) {
        super.viewDidAppear(animated)
    }

    override func touchesBegan(_ touches: Set<UITouch>, with event: UIEvent?) {
        switch self.currentEditingMode {
        case .ink:
            swiped = false
            if let touch = touches.first {
                lastPoint = touch.location(in: self.view)
            }
        case .select:
            print("not yet implemented")
        case .eraseByPoint:
            print("erase point")
        }
    }

    override func touchesMoved(_ touches: Set<UITouch>, with event: UIEvent?) {
        switch self.currentEditingMode {
        case .ink:
            swiped = true
            if let touch = touches.first {
                let currentPoint = touch.location(in: view)
                drawLine(fromPoint: lastPoint, toPoint: currentPoint)
                lastPoint = currentPoint
            }
        case .select:
            print("not yet implemented")
        case .eraseByPoint:
            print("erase point")
        }
    }

    override func touchesEnded(_ touches: Set<UITouch>, with event: UIEvent?) {
        switch self.currentEditingMode {
        case .ink:
            if !swiped {
                // draw a single point
                self.drawLine(fromPoint: lastPoint, toPoint: lastPoint)
            }
        case .select:
            print("not yet implemented")
        case .eraseByPoint:
            print("erase point")
        }
    }

    func updateEditingMode(mode: PixelEditingMode) {
         self.setEditingMode(mode: mode)
    }

    func setEditingMode(mode: PixelEditingMode) {
        switch mode {
        case .ink:
            self.currentEditingMode = PixelEditingMode.ink
            print("ink")
        case .select:
            self.currentEditingMode = PixelEditingMode.select
            print("select")
        case .eraseByPoint:
            self.currentEditingMode = PixelEditingMode.eraseByPoint
            print("erase point")
        }
    }

    func drawLine(fromPoint: CGPoint, toPoint: CGPoint) {
        UIGraphicsBeginImageContextWithOptions(view.bounds.size, false, 0)
        red = CGFloat(drawingSettingsView.redValue)
        green = CGFloat(drawingSettingsView.greenValue)
        blue = CGFloat(drawingSettingsView.blueValue)
        opacity = CGFloat(drawingSettingsView.alphaValue)
        brushWidth = CGFloat (drawingSettingsView.widthValue)
        imageView.image?.draw(in: view.bounds)
        let context = UIGraphicsGetCurrentContext()

        context?.move(to: fromPoint)
        context?.addLine(to: toPoint)

        context?.setLineCap(CGLineCap.round)
        context?.setLineWidth(brushWidth)
        context?.setStrokeColor(red: red/255, green: green/255, blue: blue/255, alpha: opacity/100)
        context?.setBlendMode(CGBlendMode.normal)
        context?.strokePath()

        imageView.image = UIGraphicsGetImageFromCurrentImageContext()
        imageView.alpha = opacity
        UIGraphicsEndImageContext()
    }
}
