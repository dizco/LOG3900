import UIKit
import Starscream

class ViewController: UIViewController, SocketManagerDelegate {
    // MARK: - Properties
    //variables
    var chatShowing = false //value to keep track of the chat window state
    var rowNumber: Int = 0
    //Labels
    @IBOutlet weak var welcomeLabel: UILabel!
    @IBOutlet weak var connexionErrorLabel: UILabel!
    //views
    @IBOutlet var placeHolderView: UIView!
    @IBOutlet weak var connexionView: UIView?
    @IBOutlet weak var registerView: UIView?
    @IBOutlet var drawView: UIView!
    @IBOutlet weak var chatView: UIView!
    @IBOutlet weak var selectorView: UIView!
    @IBOutlet weak var serverInformationsView: UIView!
    //text fields
        //server adress textfield
    @IBOutlet weak var serverAdressField: UITextField!
        //login text fields
    @IBOutlet weak var loginUsernameField: UITextField!
    @IBOutlet weak var loginPasswordField: UITextField!
        //register view text fields
    @IBOutlet weak var registerUsernameField: UITextField!
    @IBOutlet weak var registerNameField: UITextField!
    @IBOutlet weak var registerFirstNameField: UITextField!
    @IBOutlet weak var registerPasswordField: UITextField!
    @IBOutlet weak var registerPasswordValidationField: UITextField!
    
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
        let receivedAuthor = "Frederic"
        let receivedTimestamp = Timestamp()
        let messageInfos = (receivedAuthor, receivedTimestamp.getCurrentTime())


        displayMessage(message: receivedMessage!, messageInfos: messageInfos)
        do {
            let outgoingMessage = OutgoingChatMessage(message: receivedMessage!)
            let encodedData = try JSONEncoder().encode(outgoingMessage)
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
        let messageInfo = messageInfos.author + " " + messageInfos.timestamp
        authorNameMutableString = NSMutableAttributedString(string: messageInfo,
                    attributes: [NSAttributedStringKey.font: UIFont.systemFont(ofSize: 13)])
        authorNameMutableString.addAttribute(NSAttributedStringKey.font,
                                             value: UIFont.boldSystemFont(ofSize: 15),
                                             range: NSRange(location: 0, length: messageInfos.author.count) )

        titleHeading.insert(messageInfo, at: rowNumber)
        subtitleHeading.insert(message, at: rowNumber)
        chatTableView.estimatedRowHeight = 55
        chatTableView.rowHeight = UITableViewAutomaticDimension
        chatTableView.insertRows(at: [indexPath], with: .right)
        rowNumber += 1
        //the following code is to empty the text field once the message is sent
        messageField.text = ""
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
    @IBAction func serverAddressEnteredButton(_ sender: UIButton) {
        //attempt function to attempt to connect to the server modify the connexionState and errorMessage
        //expecting 2 return values, a boolean for connexionState and a string for the error message, if there is an error
        var connexionState = true
        var errorMessage: String = " "
        serverAdressEntered(connexionState: connexionState, errorMessage: errorMessage)
    }
    
    func serverAdressEntered(connexionState: Bool,  errorMessage: String) {
        if connexionState { //connection with the server established
            connexionView?.isHidden = false
            selectorView?.isHidden = false
            serverInformationsView?.isHidden = true
        }else { //error when trying to connect to the server
            connexionErrorLabel?.isHidden = false
            connexionErrorLabel?.text = "Erreur de connexion au serveur: " + errorMessage
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
        //registerView?.isHidden = true //default view is login
        //default values
        connexionView?.isHidden = true
        registerView?.isHidden = true
        selectorView?.isHidden = true
        connexionErrorLabel?.isHidden = true
        
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
            let incomingMessage = try decoder.decode(IncomingChatMessage.self, from: data)
            print(incomingMessage.message)
            let messageInfos = (incomingMessage.author.name, "hh:mm")
            displayMessage(message: incomingMessage.message, messageInfos: messageInfos)
        } catch let error {
            print(error)
        }
    }
}
