import UIKit
import Starscream

class ViewController: UIViewController {
    var chatShowing = false
    var toolsShowing = false
    @IBOutlet var drawView: UIView!
    @IBOutlet weak var chatView: UIView!
    @IBOutlet weak var toolsView: UIView!
    @IBOutlet weak var messageField: UITextField!
    @IBOutlet weak var chatViewConstraint: NSLayoutConstraint! //constraint to modify to show/hide the

    @IBOutlet weak var toolsViewConstraint: NSLayoutConstraint! //constraint to show/hide the tools view
    @IBAction func chatToggleBtn(_ sender: Any) {
        chatToggleFn()
    }

    @IBAction func toolsToggleBtn(_ sender: Any) {
        toolsToggleFn()
    }

    @IBOutlet weak var imageView: UIImageView!
    var lastPoint = CGPoint.zero //last drawn point on the canvas
    var red: CGFloat = 0.0 //RGB, stores the currend rgb value from the selector
    var green: CGFloat = 0.0
    var blue: CGFloat = 0.0
    var brushWidth: CGFloat = 10.0 //brush stroke and opacity
    var opacity: CGFloat = 1.0
    var swiped = false //if the brush stroke is continuous

    override func touchesBegan(_ touches: Set<UITouch>, with event: UIEvent?) {
        swiped = false
        if let touch = touches.first as? UITouch {
            lastPoint = touch.location(in: self.view)
        }
    }

    func drawLine(fromPoint: CGPoint, toPoint: CGPoint) {
        UIGraphicsBeginImageContextWithOptions(view.bounds.size, false, 0)

        imageView.image?.draw(in: view.bounds)

        let context = UIGraphicsGetCurrentContext()

        context?.move(to: fromPoint)
        context?.addLine(to: toPoint)

        context?.setLineCap(CGLineCap.round)
        context?.setLineWidth(brushWidth)
        context?.setStrokeColor(red: red, green: green, blue: blue, alpha: 1.0)
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

    func toolsToggleFn() {
        toolsView.layer.cornerRadius = 10
        if toolsShowing {
            toolsViewConstraint.constant = -76
            UIView.animate(withDuration: 0.3, animations: {self.view.layoutIfNeeded()})
        } else {
            toolsViewConstraint.constant = 0
            UIView.animate(withDuration: 0.3, animations: {self.view.layoutIfNeeded()})
        }
        toolsShowing = !toolsShowing
    }

    func chatToggleFn() { //function called to toggle the chat view
        let windowWidth = self.drawView.frame.width
        let chatViewWidth = self.chatView.frame.width
        if chatShowing {
            chatViewConstraint.constant = windowWidth
            UIView.animate(withDuration: 0.3, animations: {self.view.layoutIfNeeded()})
        } else {
            chatViewConstraint.constant = windowWidth - chatViewWidth
            UIView.animate(withDuration: 0.3, animations: {self.view.layoutIfNeeded()})
        }
        chatShowing = !chatShowing
    }

    override func didReceiveMemoryWarning() {
        super.didReceiveMemoryWarning()
    }

    override func viewDidLoad() {
        toolsViewConstraint.constant = -76
        super.viewDidLoad()
        self.hideKeyboard()
        observeKeyboardNotification()
    }

    fileprivate func  observeKeyboardNotification() {
        NotificationCenter.default.addObserver(self,
                                               selector: #selector(keyboardWillShow),
                                               name: NSNotification.Name.UIKeyboardWillShow,
                                               object: nil)
        NotificationCenter.default.addObserver(self,
                                               selector: #selector(keyboardWillHide),
                                               name: NSNotification.Name.UIKeyboardWillHide,
                                               object: nil)
    }

    @objc func keyboardWillShow(notification: NSNotification) {
        if let keyboardSize = (notification.userInfo?[UIKeyboardFrameBeginUserInfoKey] as? NSValue)?.cgRectValue {
            if self.view.frame.origin.y == 0 {
                self.view.frame.origin.y -= keyboardSize.height
            }
        }
    }

    @objc func keyboardWillHide(notification: NSNotification) {
        if let keyboardSize = (notification.userInfo?[UIKeyboardFrameBeginUserInfoKey] as? NSValue)?.cgRectValue {
            if self.view.frame.origin.y != 0 {
                self.view.frame.origin.y += keyboardSize.height
            }
        }
    }

    override func viewDidAppear(_ animated: Bool) {
        super.viewDidAppear(animated)
    }
}
