//
//  ViewController.swift
//  PolyPaintPro
//
//  Created by Fred Habsfan on 2018-01-16.
//  Copyright © 2018 Frederic. All rights reserved.
//

import UIKit
import Starscream

class ViewController: UIViewController {
    
    @IBOutlet weak var landScapeConstraint: NSLayoutConstraint!
    @IBOutlet weak var portrateConstraint: NSLayoutConstraint!
    
    func viewWillTransitionToSize(size: CGSize,   withTransitionCoordinator coordinator:    UIViewControllerTransitionCoordinator) {
        
        coordinator.animate(alongsideTransition: { (UIViewControllerTransitionCoordinatorContext) -> Void in
            
            let orient = UIApplication.shared.statusBarOrientation
            
            switch orient {
            case .portrait:
                print("Portrait")
                self.ApplyportraitConstraint()
                break
            // Do something
            default:
                print("LandScape")
                // Do something else
                self.applyLandScapeConstraint()
                break
            }
        }, completion: { (UIViewControllerTransitionCoordinatorContext) -> Void in
            print("rotation completed")
        })
        viewWillTransitionToSize(size: size, withTransitionCoordinator: coordinator)
    }
 
    func ApplyportraitConstraint(){
       
        self.view.addConstraint(self.portrateConstraint)
        self.view.removeConstraint(self.landScapeConstraint)
    }
    func applyLandScapeConstraint(){
        
        self.view.removeConstraint(self.portrateConstraint)
        self.view.addConstraint(self.landScapeConstraint)
    }

    // MARK: - Properties
    var username = ""
    var socket = WebSocket(url: URL(string: "ws://localhost:3000/")!, protocols: ["chat"])
    
    // MARK: - View Life Cycle
    override func viewDidLoad() {
        super.viewDidLoad()
       
        // Do any additional setup after loading the view, typically from a nib.
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
    
    public func websocketDidConnect(_ socket: Starscream.WebSocket) {
        socket.write(string: username)
    }
    
    public func websocketDidDisconnect(_ socket: Starscream.WebSocket, error: NSError?) {
        performSegue(withIdentifier: "websocketDisconnected", sender: self)
    }
    
    // Implementation will change depending on our JSON data format.
    
    /* Message format:
     * {"type":"message","data":{"time":1472513071731,"text":"😍","author":"iPhone Simulator","color":"orange"}}
     */
    public func websocketDidReceiveMessage(_ socket: Starscream.WebSocket, text: String) {
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
    
    public func websocketDidReceiveData(_ socket: Starscream.WebSocket, data: Data) {
        // Noop - Must implement since it's not optional in the protocol
    }
}
