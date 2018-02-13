import UIKit
import Starscream

class ViewController: UIViewController {
    var chatShowing = false
    @IBOutlet var drawView: UIView!
    @IBOutlet weak var chatView: UIView!
    @IBOutlet weak var messageField: UITextField!
    @IBOutlet weak var chatViewConstraint: NSLayoutConstraint! //constraint to modify to show/hide the

    @IBAction func chatToggleBtn(_ sender: Any) {
        chatToggleFn()
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
