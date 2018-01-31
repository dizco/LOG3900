import UIKit
import Starscream

class ViewController: UIViewController, UITableViewDataSource, SocketManagerDelegate {

    // MARK: - Properties
    var chatShowing = false //value to keep track of the chat window state

    @IBOutlet weak var welcomeLabel: UILabel!

    @IBOutlet weak var connexionView: UIView?
    @IBOutlet weak var registerView: UIView?

    @IBOutlet var drawView: UIView!
    @IBOutlet weak var chatView: UIView!

    @IBOutlet weak var chatViewConstraint: NSLayoutConstraint! //constraint to modify to show/hide the chat window

    @IBAction func chatToggleBtn(_ sender: Any) { //function associated to the chat toggle button
        chatToggleFn()
    }

    //number of sections in the chat table
    func numberOfSections(in tableView: UITableView) -> Int {
        return 1
    }

    //number of columns in the chat table
    func tableView(_ tableView: UITableView, numberOfRowsInSection section: Int) -> Int {
        return 3
        //return chatTable.count
        // ^ size of the table containing messages, to allow to be dynamically modified with the arrival of new messages
    }

    func tableView(_ tableView: UITableView, cellForRowAt indexPath: IndexPath) -> UITableViewCell {
        var cell = UITableViewCell()
        cell.textLabel?.text = "Bacon" //this data will be replaced by actual messages
        return cell
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
