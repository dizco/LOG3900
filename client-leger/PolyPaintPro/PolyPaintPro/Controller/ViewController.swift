import UIKit
import Starscream

class ViewController: UIViewController {
    var chatShowing = false
    var toolsShowing = false
    var drawingSettingsShowing = false
    var redValue: Int = 0
    var greenValue: Int = 0
    var  blueValue: Int = 0
    var alphaValue: Int = 100
    @IBOutlet var drawView: UIView!
    @IBOutlet weak var chatView: UIView!
    @IBOutlet weak var toolsView: UIView!
    @IBOutlet weak var drawingSettingsView: UIView!
    @IBOutlet weak var imageView: UIImageView!
    @IBOutlet weak var messageField: UITextField!
    @IBOutlet weak var redField: UITextField!
    @IBOutlet weak var greenField: UITextField!
    @IBOutlet weak var blueField: UITextField!
    @IBOutlet weak var alphaField: UITextField!
    @IBOutlet weak var redSlider: UISlider!
    @IBOutlet weak var greenSlider: UISlider!
    @IBOutlet weak var blueSlider: UISlider!
    @IBOutlet weak var alphaSlider: UISlider!
    @IBOutlet weak var chatViewConstraint: NSLayoutConstraint! //constraint to modify to show/hide the
    @IBOutlet weak var toolsViewConstraint: NSLayoutConstraint! //constraint to show/hide the tools view
    @IBOutlet weak var drawingSettingsContraint: NSLayoutConstraint! //constraint to show#hide drawing tools
    @IBAction func editDrawingSettings(_ sender: Any) {
        drawingSettingsFn()
    }
    @IBAction func chatToggleBtn(_ sender: Any) {
        chatToggleFn()
    }

    @IBAction func toolsToggleBtn(_ sender: Any) {
        toolsToggleFn()
        if drawingSettingsShowing {
            drawingSettingsFn()
        }
    }

    var lastPoint = CGPoint.zero //last drawn point on the canvas
    var red: CGFloat = 0.0 //RGB, stores the currend rgb value from the selector
    var green: CGFloat = 0.0
    var blue: CGFloat = 0.0
    var brushWidth: CGFloat = 10.0 //brush stroke and opacity
    var opacity: CGFloat = 1.0
    var swiped = false //if the brush stroke is continuous

    @IBAction func redSliderChanged(_ sender: UISlider) {
        redValue = lroundf(sender.value)
        redField.text! = "\(redValue)"
    }
    @IBAction func greenSliderChanged(_ sender: UISlider) {
        greenValue = lroundf(sender.value)
        greenField.text! = "\(greenValue)"
    }
    @IBAction func blueSliderChanged(_ sender: UISlider) {
        blueValue = lroundf(sender.value)
        blueField.text! = "\(blueValue)"
    }
    @IBAction func alphaSliderChanged(_ sender: UISlider) {
        alphaValue = lroundf(sender.value)
        alphaField.text! = "\(alphaValue)"
    }
    @IBAction func redTextFieldChanged(_ sender: UITextField) {
        redValue = (redField.text as! NSString).integerValue
        redSlider.value = Float(redValue)
    }
    @IBAction func greenTextFieldChanged(_ sender: UITextField) {
        greenValue = (greenField.text as! NSString).integerValue
        greenSlider.value = Float(greenValue)
    }
    @IBAction func blueTextFieldChanged(_ sender: Any) {
        blueValue = (blueField.text as! NSString).integerValue
        blueSlider.value = Float(blueValue)
    }
    @IBAction func alphaTextFieldChanged(_ sender: UITextField) {
        alphaValue = (alphaField.text as! NSString).integerValue
        alphaSlider.value = Float(alphaValue)
    }

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
        let sidebarWidth = self.toolsView.frame.width
        if toolsShowing {
            toolsViewConstraint.constant = -sidebarWidth
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

    func drawingSettingsFn() {
        let settingsView = self.drawingSettingsView.frame.width
        if drawingSettingsShowing {
            drawingSettingsContraint.constant = -settingsView
            UIView.animate(withDuration: 0.3, animations: {self.view.layoutIfNeeded()})
        } else {
            drawingSettingsContraint.constant = 0
            UIView.animate(withDuration: 0.3, animations: {self.view.layoutIfNeeded()})
        }
        drawingSettingsShowing = !drawingSettingsShowing
    }

    override func didReceiveMemoryWarning() {
        super.didReceiveMemoryWarning()
    }

    override func viewDidLoad() {
        toolsViewConstraint.constant = -self.toolsView.frame.width
        drawingSettingsContraint.constant = -self.drawingSettingsView.frame.width
        toolsView.layer.cornerRadius = 10
        drawingSettingsView.layer.cornerRadius = 10
        redField.text! = "\(redValue)"
        redSlider.value = Float(redValue)
        greenField.text! = "\(greenValue)"
        greenSlider.value = Float(greenValue)
        blueField.text! = "\(blueValue)"
        blueSlider.value = Float(blueValue)
        alphaField.text! = "\(alphaValue)"
        alphaSlider.value = Float(alphaValue)
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
