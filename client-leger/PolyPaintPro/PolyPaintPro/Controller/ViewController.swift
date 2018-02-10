import UIKit
import Starscream

class ViewController: UIViewController, SocketManagerDelegate {
    // MARK: - Properties
    //variables
    var chatShowing = false //value to keep track of the chat window state
    var rowNumber: Int = 0
    //views
    @IBOutlet var drawView: UIView!
    @IBOutlet weak var chatView: UIView!
        //chat view text field
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
        let receivedAuthor = AccountManager.sharedInstance.username!
        let receivedTimestamp = Timestamp()
        let messageInfos = (receivedAuthor, receivedTimestamp.getCurrentTime())

        if !receivedMessage!.isEmpty {
            displayMessage(message: receivedMessage!, messageInfos: messageInfos)
            do {
                let outgoingMessage = OutgoingChatMessage(message: receivedMessage!)
                let encodedData = try JSONEncoder().encode(outgoingMessage)
                SocketManager.sharedInstance.send(data: encodedData)
            } catch let error {
                print(error)
            }
        }
    }

    //function to call to add a new message in the chat
    func displayMessage(message: String, messageInfos: (author: String, timestamp: String)) {
        
        //the following code is to add messages to the chat view
        let indexPath = IndexPath.init(row: rowNumber, section: 0)
        
        //the following code formats the message for display
        let messageInfo = messageInfos.author + " " + messageInfos.timestamp
        authorNameMutableString = NSMutableAttributedString(string: messageInfo,
                                                            attributes: [NSAttributedStringKey.font: UIFont.systemFont(ofSize: 13)])
        authorNameMutableString.addAttribute(NSAttributedStringKey.font,
                                             value: UIFont.boldSystemFont(ofSize: 15),
                                             range: NSRange(location: 0, length: messageInfos.author.count) )
        
        titleHeading.insert(messageInfo, at: rowNumber)
        subtitleHeading.insert(message, at: rowNumber)
        chatTableView.estimatedRowHeight = 55
        updateContentInsetForTableView(tableView: chatTableView, animated: true)
        chatTableView.rowHeight = UITableViewAutomaticDimension
        chatTableView.insertRows(at: [indexPath], with: .right)
        rowNumber += 1
        messageField.text = ""
    }
    
     func updateContentInsetForTableView( tableView:UITableView,animated:Bool) {
        let lastRow = tableView.numberOfRows(inSection: 0)
        let lastIndex = lastRow > 0 ? lastRow - 1 : 0
        let lastIndexPath = IndexPath(row: lastIndex, section: 9)
        let lastCellFrame = tableView.rectForRow(at: lastIndexPath)
        let topInset = max(tableView.frame.height - lastCellFrame.origin.y - lastCellFrame.height, 0)
        var contentInset = tableView.contentInset
        contentInset.top = topInset
        _ = UIViewAnimationOptions.beginFromCurrentState
        UIView.animate(withDuration: 0.1, animations: { () -> Void in
            tableView.contentInset = contentInset
     }
        )
     
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

    // MARK: - Memory Warning
    override func didReceiveMemoryWarning() {
        super.didReceiveMemoryWarning()
        // Dispose of any resources that can be recreated.
    }

    // MARK: - View Life Cycle
    override func viewDidLoad() {
        super.viewDidLoad()
        //registerView?.isHidden = true //default view is login
        //default values

        self.hideKeyboard()

        observeKeyboardNotification()
        // TO-MOVE: Initialize socket only in ChatViewController
        SocketManager.sharedInstance.delegate = self
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
            let incomingMessage = try decoder.decode(IncomingChatMessage.self, from: data)
            let convertTime = Timestamp()
            let timestamp = convertTime.getTimeFromServer(timestamp: incomingMessage.timestamp)
            let messageInfos = (incomingMessage.author.name, timestamp)
            displayMessage(message: incomingMessage.message, messageInfos: messageInfos)
        } catch let error {
            print(error)
        }
    }
}
