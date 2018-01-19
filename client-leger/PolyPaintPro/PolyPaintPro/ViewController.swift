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
    
    // MARK: - Properties
    var username = ""
    let socket = WebSocket(url: URL(string: "ws://localhost:3000/")!)
    
    // MARK: - View Life Cycle
    override func viewDidLoad() {
        super.viewDidLoad()
       
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
        performSegue(withIdentifier: "websocketDisconnected", sender: self)
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
    
    
   ///////////////////////////////////////////////////////////////////////////////
  /*
    class helper{
        static func lockOrientation(_ orientation: UIInterfaceOrientationMask) {
            if let delegate = UIApplication.shared.delegate as? AppDelegate {
                delegate.orientationLock = orientation
            }
        }
        /// OPTIONAL Added method to adjust lock and rotate to the desired orientation
        static func lockOrientation(_ orientation: UIInterfaceOrientationMask, andRotateTo rotateOrientation:UIInterfaceOrientation) {
            self.lockOrientation(orientation)
            UIDevice.current.setValue(rotateOrientation.rawValue, forKey: "orientation")
        }
    }
    
    
    override func viewDidAppear(_ animated: Bool) {
        super.viewDidAppear(animated)
        helper.lockOrientation(.landscapeRight)
    }
    
    override func viewWillDisappear(_ animated: Bool) {
        super.viewWillDisappear(animated)
        helper.lockOrientation(.all)
    }
    
    */
}
