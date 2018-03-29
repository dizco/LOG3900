import UIKit
import Starscream
import SpriteKit

enum PixelEditingMode {
    case ink, select, eraseByPoint
}

class PixelEditorViewController: EditorViewController, PixelToolsViewDelegate {
    internal var lastPoint = CGPoint.zero //last drawn point on the canvas
    internal var fisrtPointSelection = CGPoint.zero
    internal var lastPointSelection = CGPoint.zero
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
        toolsView.pixelDelegate = self
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
            swiped = false
            if let touch = touches.first {
                fisrtPointSelection = touch.location(in: self.view)
                print(fisrtPointSelection)
            }
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
            swiped = true
            if let touch = touches.first {
                let currentPoint = touch.location(in: view)
                selectArea(fromPoint: fisrtPointSelection, toPoint: currentPoint)
                lastPoint = currentPoint
            }
        case .eraseByPoint:
            print("erase point")
        }
    }

    override func touchesEnded(_ touches: Set<UITouch>, with event: UIEvent?) {
        switch self.currentEditingMode {
        case .ink:
            swiped = false
            if let touch = touches.first {
                lastPoint = touch.location(in: self.view)
            }
        case .select:
            swiped = false
            if let touch = touches.first {
                lastPointSelection = touch.location(in: self.view)
            }
            drawSelectionRectangle(fromPoint: fisrtPointSelection, toPoint: lastPointSelection)
        case .eraseByPoint:
            print("erase point")
        }
    }

    func updateEditingMode(mode: PixelEditingMode) {
         self.currentEditingMode = mode
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

    func selectArea(fromPoint: CGPoint, toPoint: CGPoint) {
        drawSelectionRectangle(fromPoint: fisrtPointSelection, toPoint: toPoint)
    }

    func drawSelectionRectangle(fromPoint: CGPoint, toPoint: CGPoint) {
        UIGraphicsBeginImageContextWithOptions(view.bounds.size, false, 0)
        imageView.image?.draw(in: view.bounds)
        let context = UIGraphicsGetCurrentContext()
        context?.setLineWidth(4.0)
        context?.setStrokeColor(UIColor.blue.cgColor)
        let rectangle = CGRect(x: fromPoint.x, y: fromPoint.y, width: toPoint.x - fromPoint.x, height: toPoint.y - fromPoint.y)
        context?.addRect(rectangle)
        context?.strokePath()

        imageView.image = UIGraphicsGetImageFromCurrentImageContext()
        imageView.alpha = opacity
        UIGraphicsEndImageContext()
    }
}
