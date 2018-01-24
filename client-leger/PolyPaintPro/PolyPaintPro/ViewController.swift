
import UIKit
import Starscream

class ViewController: UIViewController, UITableViewDataSource {
    
    // MARK: - Properties
    var username = ""
    let socket = WebSocket(url: URL(string: "ws://localhost:3000/")!)
    var chatShowing = false //value to keep track of the chat window state

    @IBOutlet weak var welcomeLabel: UILabel!
    
    @IBOutlet weak var connexionView: UIView!
    @IBOutlet weak var registerView: UIView?
    
    @IBOutlet weak var chatViewConstraint: NSLayoutConstraint! //constraint to modify to show/hide the chat window
    
    @IBAction func chatToggleBtn(_ sender: Any) { //function associated to the button
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
        // ^ size of the table containing messages, to allow to be dynamically modified woth the arrrival of new messages
    }
    
    func tableView(_ tableView: UITableView, cellForRowAt indexPath: IndexPath) -> UITableViewCell {
        var cell = UITableViewCell()
        cell.textLabel?.text = "Bacon" //this data will be replaced by actual messages
        return cell
    }
    
    
    func chatToggleFn(){ //function called to toggle the chat view
        if(chatShowing){
            chatViewConstraint.constant = 1024
            UIView.animate(withDuration: 0.3, animations: {self.view.layoutIfNeeded()})
            
            
        }else{
            chatViewConstraint.constant = 768
            UIView.animate(withDuration: 0.3, animations: {self.view.layoutIfNeeded()})
            
            
        }
        chatShowing = !chatShowing
        
    }
    
    @IBAction func loginToggle(_ sender: UISegmentedControl) {
        if(sender.selectedSegmentIndex == 0){
            welcomeLabel.text = "Bienvenue! Entrez vos informations de connexion PolyPaintPro"
            connexionView.isHidden = false
            registerView?.isHidden = true
        }else if(sender.selectedSegmentIndex == 1){
            welcomeLabel.text = "Bienvenue! Entrez vos informations pour creer un compte PolyPaint Pro"
            connexionView.isHidden = true
            registerView?.isHidden = false

        }
    }
    
    // MARK: - View Life Cycle
    override func viewDidLoad() {
       
        super.viewDidLoad()
        registerView?.isHidden = true //default view is login
       
        socket.delegate = self
        socket.connect()
    }
    
    deinit {
        socket.disconnect(forceTimeout: 0)
        socket.delegate = nil
    }

    override func didReceiveMemoryWarning() {
        super.didReceiveMemoryWarning()
        // Dispose of any resources that can be recreated.
    }
}

// MARK: - FilePrivate
extension ViewController {
    
    fileprivate func sendMessage(_ message: String) {
        socket.write(string: message)
    }
    
    fileprivate func messageReceived(_ message: String, senderName: String) {
        // TO-DO : Receive a message.
    }
}

// Source : https://www.raywenderlich.com/143874/websockets-ios-starscream
// MARK: - WebSocketDelegate
extension ViewController : WebSocketDelegate {
    public func websocketDidConnect(socket: WebSocketClient) {
        socket.write(string: username)
    }
    
    public func websocketDidDisconnect(socket: WebSocketClient, error: Error?) {
        //performSegue(withIdentifier: "websocketDisconnected", sender: self)
    }
    
    // WIll change depending on JSON data format.
    
    /* Message format:
     * {"type":"message","data":{"time":1472513071731,"text":"😍","author":"iPhone Simulator","color":"orange"}}
     */
    
    public func websocketDidReceiveMessage(socket: WebSocketClient, text: String) {
        guard let data = text.data(using: .utf16),
            let jsonData = try? JSONSerialization.jsonObject(with: data),
            let jsonDict = jsonData as? [String: Any],
            let messageType = jsonDict["type"] as? String else {
                return
        }
        
        if messageType == "message",
            let messageData = jsonDict["data"] as? [String: Any],
            let messageAuthor = messageData["author"] as? String,
            let messageText = messageData["text"] as? String {
            
            messageReceived(messageText, senderName: messageAuthor)
        }
    }
    
    public func websocketDidReceiveData(socket: WebSocketClient, data: Data) {
        // Noop - Must implement since it's not optional in the protocol
    }
}
