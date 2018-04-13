import UIKit
import Starscream

enum PixelEditingMode {
    case ink, select, eraseByPoint, filter
}

class PixelEditorViewController: EditorViewController, ActionSocketManagerDelegate, PixelToolsViewDelegate {
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

/////////////////////////////////////////////////////// filter

    @IBOutlet var filtersToolsView: FilterToolsView!
    @IBOutlet var filtersToolsViewConstraint: NSLayoutConstraint!
    var filtersToolsShowing = false

    override func toggleFiltersToolsView () {
        let filtersViewWidth = self.filtersToolsView.frame.width
        if !toolsShowing {
            filtersToolsViewConstraint.constant = -filtersViewWidth
        } else {
            filtersToolsViewConstraint.constant = 0
            //toolsView.setDefault()
        }
        UIView.animate(withDuration: 0.3, animations: {self.view.layoutIfNeeded()})
        toolsShowing = !toolsShowing
    }



    /////////////////////////////////////// filters


    override func didReceiveMemoryWarning() {
        super.didReceiveMemoryWarning()
    }

    override func viewDidLoad() {
        super.viewDidLoad()
        toolsView.pixelDelegate = self
        SocketManager.sharedInstance.actionDelegate = self
        filtersToolsView.layer.cornerRadius = 10
        filtersToolsViewConstraint.constant = -self.filtersToolsView.frame.width
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
        case .filter:
            print("filter seleected")
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
        case .filter:
            print("filter seleected")

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
        case .filter:
            print("filter seleected")

        }
    }

    func updateEditingMode(mode: PixelEditingMode) {
         self.currentEditingMode = mode
    }

    func drawLine(fromPoint: CGPoint, toPoint: CGPoint) {
        UIGraphicsBeginImageContextWithOptions(self.view.bounds.size, false, 0)
        self.red = CGFloat(drawingSettingsView.redValue / 255)
        self.green = CGFloat(drawingSettingsView.greenValue / 255)
        self.blue = CGFloat(drawingSettingsView.blueValue / 255)
        self.opacity = CGFloat(drawingSettingsView.alphaValue / 100)
        self.brushWidth = CGFloat(drawingSettingsView.widthValue)
        self.imageView.image?.draw(in: view.bounds)
        let context = UIGraphicsGetCurrentContext()

        context?.move(to: fromPoint)
        context?.addLine(to: toPoint)

        context?.setLineCap(CGLineCap.round)
        context?.setLineWidth(self.brushWidth)
        context?.setAlpha(self.opacity)
        // Because we set the overall alpha earlier, the stroke color's alpha must be at 1.0
        // Else, both values interact with each other.
        context?.setStrokeColor(red: self.red, green: self.green, blue: self.blue, alpha: 1.0)
        context?.setBlendMode(CGBlendMode.normal)
        context?.strokePath()

        self.imageView.image = UIGraphicsGetImageFromCurrentImageContext()
        UIGraphicsEndImageContext()

        // Send the pixel online
        if SocketManager.sharedInstance.getConnectionStatus() {
            let color = SKStrokeColor(red: self.red, green: self.green, blue: self.blue, alpha: self.opacity)
            let pixel1 = UIPixel(point: fromPoint, color: color)
            let pixel2 = UIPixel(point: toPoint, color: color)
            self.sendEditorAction(actionId: PixelActionIdConstants.add.rawValue, fromPixel: pixel1, toPixel: pixel2)
        }
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

    // MARK: - Functions for sending pixels
    func sendEditorAction(actionId: Int, fromPixel: UIPixel, toPixel: UIPixel) {
        // Only send if the socket is connected
        if SocketManager.sharedInstance.getConnectionStatus() {
            do {
                let builtAction = BuildPixelActionStrategyContext(viewController: self, actionId: actionId, fromPixel: fromPixel, toPixel: toPixel)
                let outgoingActionMessage = builtAction.getOutgoingMessage()
                let encodedData = try JSONEncoder().encode(outgoingActionMessage)
                SocketManager.sharedInstance.send(data: encodedData)
            } catch let error {
                print(error)
            }
        }
    }

    // MARK: - Functions for receiving pixels
    private func applyReceived(incomingAction: IncomingPixelActionMessage) {
        _ = PixelActionStrategyContext(viewController: self, incomingAction: incomingAction)
    }

    // MARK: - ActionSocketManagerDelegate
    func managerDidReceiveAction(data: Data) {
        do {
            print("Action data received.")
            let decoder = JSONDecoder()
            let incomingAction = try decoder.decode(IncomingPixelActionMessage.self, from: data)
            self.applyReceived(incomingAction: incomingAction)
        } catch let error {
            print(error)
        }
    }
}
