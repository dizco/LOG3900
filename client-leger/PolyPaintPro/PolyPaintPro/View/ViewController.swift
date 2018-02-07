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

    var titleHeading: [String] = [""]
    var subtitleHeading: [String] = [""]
    var authorNameMutableString = NSMutableAttributedString()
    @IBOutlet weak var chatTableView: UITableView!
    @IBAction func sendButton(_ sender: UIButton) {
        let receivedMessage = messageField!.text
        let receivedAuthor = "Frederic"
        let receivedTimestamp = Timestamp()
        let msgInfos = (receivedAuthor, receivedTimestamp.getCurrentTime())

        displayMessage(message: receivedMessage!, messageInfos: msgInfos)
        do {
            let outgoingMsg = OutgoingChatMessage(message: receivedMessage!)
            let encodedData = try JSONEncoder().encode(outgoingMsg)
            SocketManager.sharedInstance.send(data: encodedData)
        } catch let error {
            print(error)
        }
    }
    //function to call to add a new message in the chat
    func displayMessage(message: String, messageInfos: (author: String, timestamp: String)) {

        //the following code is to add messages to the chat view
        let indexPath = IndexPath.init(row: rowNumber, section: 0)

        //the following code formats the message for display
        let msgInfo = messageInfos.author + " " + messageInfos.timestamp
        authorNameMutableString = NSMutableAttributedString(string: msgInfo,
                                                            attributes: [NSAttributedStringKey.font: UIFont.systemFont(ofSize: 13)])
        authorNameMutableString.addAttribute(NSAttributedStringKey.font,
                                             value: UIFont.boldSystemFont(ofSize: 15),
                                             range: NSRange(location: 0, length: messageInfos.author.count) )

        titleHeading.insert(msgInfo, at: rowNumber)
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

        NotificationCenter.default.addObserver(self,
                                               selector: #selector(ViewController.keyboardWillShow),
                                               name: NSNotification.Name.UIKeyboardWillShow,
                                               object: nil)
        NotificationCenter.default.addObserver(self,
                                               selector: #selector(ViewController.keyboardWillHide),
                                               name: NSNotification.Name.UIKeyboardWillHide,
                                               object: nil)

        // TO-MOVE: Initialize socket only in ChatViewController
        SocketManager.sharedInstance.delegate = self
    }

    override func viewDidAppear(_ animated: Bool) {
        super.viewDidAppear(animated)

        // TO-MOVE: Connect with socket only in ChatViewController
        SocketManager.sharedInstance.establishConnection()
    }

    // MARK: - SocketManagerDelegate
    // TO-MOVE: Isolate in a separate ViewController later
    func connect() {
        print("Connecting to server.")
    }

    // TO-MOVE: Isolate in a separate ViewController later
    func disconnect(error: Error?) {
        print ("Disconnected with error: \(String(describing: error?.localizedDescription))")
    }

    // TO-MOVE: Isolate in a separate ViewController later
    func managerDidReceive(data: Data) {
        do {
            print("Data received.")
            let decoder = JSONDecoder()
            let incomingMsg = try decoder.decode(IncomingChatMessage.self, from: data)
            print(incomingMsg.message)
            let msgInfos = (incomingMsg.author.name, "hh:mm")
            displayMessage(message: incomingMsg.message, messageInfos: msgInfos)
        } catch let error {
            print(error)
        }
    }
}
