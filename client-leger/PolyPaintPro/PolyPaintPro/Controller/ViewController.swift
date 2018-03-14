import UIKit
import Starscream
import SpriteKit

class ViewController: UIViewController {
    var chatShowing = false
    var toolsShowing = false
    var drawingSettingsShowing = false
    var connectionStatus = true
    var drawingType = ""
    
    @IBOutlet var drawView: UIView!
    @IBOutlet weak var chatView: UIView!
    @IBOutlet weak var toolsView: ToolsView!
    @IBOutlet weak var drawingSettingsView: DrawingToolsView!
    @IBOutlet weak var imageView: UIImageView!
    @IBOutlet weak var messageField: UITextField!
    @IBOutlet weak var chatViewConstraint: NSLayoutConstraint! //constraint to modify to show/hide the
    @IBOutlet weak var toolsViewConstraint: NSLayoutConstraint! //constraint to show/hide the tools view
    @IBOutlet weak var drawingSettingsContraint: NSLayoutConstraint! //constraint to show#hide drawing tools
    @IBAction func editDrawingSettings(_ sender: Any) {
        drawingSettingsFn()
    }
    @IBAction func chatToggleBtn(_ sender: Any) {
        chatToggleFn()
    }

    @IBOutlet weak var chatToggleBtn: UIBarButtonItem!
    @IBAction func toolsToggleBtn(_ sender: Any) {
        toolsToggleFn()
        if drawingSettingsShowing {
            drawingSettingsFn()
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
            toolsView.setDefault()
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
        if (drawingType == "Mode par trait"){
            //commands to load the spritekit elements
            let scene = StrokeEditorScene(size: view.frame.size)
            let skView = view as! SKView
            scene.scaleMode = .fill
            skView.presentScene(scene)
        } else if (drawingType == "Mode par pixel"){
            //commands to load core graphics elements           
        } else {
            print("Y A DE QUOI DE CASSÉ TABARNAK")
        }

        toolsViewConstraint.constant = -self.toolsView.frame.width
        drawingSettingsContraint.constant = -self.drawingSettingsView.frame.width
        toolsView.layer.cornerRadius = 10
        drawingSettingsView.layer.cornerRadius = 10
        super.viewDidLoad()
        self.hideKeyboard()
        observeKeyboardNotification()
        PixelEditorViewController() 
        if(!connectionStatus){
            chatToggleBtn.isEnabled = false
        }
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
