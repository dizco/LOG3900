
import UIKit
import Starscream

class ViewController: UIViewController {
    
    // MARK: - Properties
    var username = ""
    let socket = WebSocket(url: URL(string: "ws://localhost:3000/")!)
    
    
    
    
    @IBOutlet weak var welcomeLabel: UILabel!
    
    
    @IBOutlet weak var connexionView: UIView!
    @IBOutlet weak var registerView: UIView!
    
    @IBAction func loginToggle(_ sender: UISegmentedControl) {
        if(sender.selectedSegmentIndex == 0){
            welcomeLabel.text = "Bienvenue! Entrez vos informations de connexion PolyPaintPro"
            connexionView.isHidden = false
            registerView.isHidden = true
        }else if(sender.selectedSegmentIndex == 1){
            welcomeLabel.text = "Bienvenue! Entrez vos informations pour creer un compte PolyPaint Pro"
            connexionView.isHidden = true
            registerView.isHidden = false

        }
    }
    
    
    // MARK: - View Life Cycle
    override func viewDidLoad() {
        super.viewDidLoad()
        
        registerView.isHidden = true
       
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
