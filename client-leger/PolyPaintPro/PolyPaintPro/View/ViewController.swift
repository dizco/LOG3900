import UIKit
import Starscream

class ViewController: UIViewController, SocketManagerDelegate {
    // MARK: - Properties
    //variables
    var chatShowing = false //value to keep track of the chat window state
    var rowNumber: Int = 0
    //Labels
    @IBOutlet weak var welcomeLabel: UILabel!
    //views
    @IBOutlet weak var connexionView: UIView?
    @IBOutlet weak var registerView: UIView?
    @IBOutlet var drawView: UIView!
    @IBOutlet weak var chatView: UIView!
    //text fields
    @IBOutlet weak var messageField: UITextField!
    //Constraints
    @IBOutlet weak var chatViewConstraint: NSLayoutConstraint! //constraint to modify to show/hide the chat window

    //Buttons
    @IBAction func chatToggleBtn(_ sender: Any) { //function associated to the chat toggle button
        chatToggleFn()
    }
    ///////////////chat table view reserved section
    var titleHeading: [String] = [""]
    var subtitleHeading: [String] = [""]
    var authorNameMutableString = NSMutableAttributedString()
    @IBOutlet weak var chatTableView: UITableView!
    @IBAction func sendButton(_ sender: UIButton) {
        let receivedMessage = String(describing: messageField.text)
        let receivedAuthor = "Frederic"
        let receivedTimeStamp = "hh:mm"
        let msgInfos = receivedAuthor + " " + receivedTimeStamp
        authorNameMutableString = NSMutableAttributedString(string: msgInfos, attributes: [NSAttributedStringKey.font:UIFont.systemFont(ofSize: 13)])
        authorNameMutableString.addAttribute(NSAttributedStringKey.font, value: UIFont.boldSystemFont(ofSize: 15), range:NSRange(location: 0, length: receivedAuthor.count) )
        displayMessage(message: receivedMessage, messageInfos: msgInfos)
    }
    //function to call to add a new message in the chat
    func displayMessage(message: String, messageInfos: String) {
        //the following code is to add messages to the chat view
        let indexPath = IndexPath.init(row: rowNumber, section: 0)
        titleHeading.insert(messageInfos, at: rowNumber)
        subtitleHeading.insert(message, at: rowNumber)
        chatTableView.estimatedRowHeight = 55
        chatTableView.rowHeight = UITableViewAutomaticDimension
        chatTableView.insertRows(at: [indexPath], with: .right)
        rowNumber += 1
        //the following code is to empty the text field once the message is sent
        messageField.text = ""
    }
    
    @objc func keyboardWillShow(notification: NSNotification) {
        if let keyboardSize = (notification.userInfo?[UIKeyboardFrameBeginUserInfoKey] as? NSValue)?.cgRectValue {
            if self.view.frame.origin.y == 0{
                self.view.frame.origin.y -= keyboardSize.height
            }
        }
    }
    
    @objc func keyboardWillHide(notification: NSNotification) {
        if let keyboardSize = (notification.userInfo?[UIKeyboardFrameBeginUserInfoKey] as? NSValue)?.cgRectValue {
            if self.view.frame.origin.y != 0{
                self.view.frame.origin.y += keyboardSize.height
            }
        }
    }
    
    ///////////////chat table view reserved section
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

    @IBAction func loginToggle(_ sender: UISegmentedControl) {
        if sender.selectedSegmentIndex == 0 {
            welcomeLabel.text = "Bienvenue! Entrez vos informations de connexion PolyPaintPro"
            connexionView?.isHidden = false
            registerView?.isHidden = true
        } else if sender.selectedSegmentIndex == 1 {
            welcomeLabel.text = "Bienvenue! Entrez vos informations pour creer un compte PolyPaint Pro"
            connexionView?.isHidden = true
            registerView?.isHidden = false
        }
    }

    // MARK: - Memory Warning
    override func didReceiveMemoryWarning() {
        super.didReceiveMemoryWarning()
        // Dispose of any resources that can be recreated.
    }

    // MARK: - View Life Cycle
    override func viewDidLoad() {
        super.viewDidLoad()
        registerView?.isHidden = true //default view is login
        self.hideKeyboard()
        
        NotificationCenter.default.addObserver(self, selector: #selector(ViewController.keyboardWillShow), name: NSNotification.Name.UIKeyboardWillShow, object: nil)
        NotificationCenter.default.addObserver(self, selector: #selector(ViewController.keyboardWillHide), name: NSNotification.Name.UIKeyboardWillHide, object: nil)
        
        // TO-MOVE: Initialize socket only in ChatViewController
        SocketManager.sharedInstance.delegate = self
    }

    override func viewDidAppear(_ animated: Bool) {
        super.viewDidAppear(animated)

        // TO-MOVE: Connect with socket only in ChatViewController
        SocketManager.sharedInstance.establishConnection()

        // This call is used as a test for sending messages.
        connect()
    }

    // MARK: - SocketManagerDelegate
    // TO-MOVE: Isolate in a separate ViewController later
    func connect() {
        print("Connecting to server.")
        let message = "J'aime les Pods sur mes tartines le matin."
        let sentMsg = MessageFactory.message(for: .server, fromServer: ["": ""])?.createJSON(withMsg: message)
        do {
            let data = try JSONSerialization.data(withJSONObject: sentMsg, options: .prettyPrinted)
            SocketManager.sharedInstance.send(data: data)
        } catch {
            print("Couldn't connect to the server due to an unknown error.")
        }
    }

    // TO-MOVE: Isolate in a separate ViewController later
    func disconnect(error: Error?) {
        // TO-DO: Correct error handling.
        print ("Disconnected with error:")
    }

    // TO-MOVE: Isolate in a separate ViewController later
    func managerDidReceive(data: Data) {
        do {
            // TO-DO: Verify if it's possible to not rely on force casts.
            // swiftlint:disable force_cast
            let json = try JSONSerialization.jsonObject(with: data, options: .allowFragments) as! [String: Any]
            // swiftlint:enable force_cast
            print(json)

            // TO-DO: Use those info for something.
            let chatBubble = MessageFactory.message(for: .client, fromServer: json)
        } catch let error {
            print(error)
        }
    }
}


