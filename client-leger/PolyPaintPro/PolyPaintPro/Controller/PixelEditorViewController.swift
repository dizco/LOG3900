import UIKit
import Starscream
import SpriteKit

class PixelEditorViewController: EditorViewController {
    internal var lastPoint = CGPoint.zero //last drawn point on the canvas
    internal var red: CGFloat = 0.0 //RGB, stores the currend rgb value from the selector
    internal var green: CGFloat = 0.0
    internal var blue: CGFloat = 0.0
    internal var brushWidth: CGFloat = 10.0 //brush stroke and opacity
    internal var opacity: CGFloat = 1.0
    internal var swiped = false //if the brush stroke is continuous

    override func didReceiveMemoryWarning() {
        super.didReceiveMemoryWarning()
    }

    override func viewDidLoad() {
        super.viewDidLoad()
    }

    override func viewDidAppear(_ animated: Bool) {
        super.viewDidAppear(animated)
    }

    override func touchesBegan(_ touches: Set<UITouch>, with event: UIEvent?) {
        swiped = false
        if let touch = touches.first {
            lastPoint = touch.location(in: self.view)
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

    override func touchesMoved(_ touches: Set<UITouch>, with event: UIEvent?) {
        swiped = true
        if let touch = touches.first {
            let currentPoint = touch.location(in: view)
            drawLine(fromPoint: lastPoint, toPoint: currentPoint)
            lastPoint = currentPoint
        }
    }

    override func touchesEnded(_ touches: Set<UITouch>, with event: UIEvent?) {
        if !swiped {
            // draw a single point
            self.drawLine(fromPoint: lastPoint, toPoint: lastPoint)
        }
    }

}
