//
//  EditorViewController.swift
//  PolyPaintPro
//
//  Created by Gabriel Bourgault on 2018-03-14.
//  Copyright © 2018 Les Pods c'est pour les lunchs. All rights reserved.
//

import Foundation
import SpriteKit
import AVFoundation

class EditorViewController: UIViewController, SocketManagerDelegate, UITextFieldDelegate {
    private var timer: Timer!
    internal var chatShowing = false
    internal var toolsShowing = false
    internal var drawingSettingsShowing = false
    internal var connectionStatus = true

    @IBOutlet weak var drawView: UIView!
    @IBOutlet weak var chatView: ChatView!
    @IBOutlet weak var toolsView: ToolsView!
    @IBOutlet weak var drawingSettingsView: DrawingToolsView!
    @IBOutlet weak var imageView: UIImageView!
    @IBOutlet weak var messageField: UITextField!
    @IBOutlet weak var chatToggleBtn: UIBarButtonItem!
    @IBOutlet weak var chatViewConstraint: NSLayoutConstraint! //constraint to modify to show/hide the
    @IBOutlet weak var toolsViewConstraint: NSLayoutConstraint! //constraint to show/hide the tools view
    @IBOutlet weak var drawingSettingsContraint: NSLayoutConstraint! //constraint to show/hide drawing tools

    override func didReceiveMemoryWarning() {
        super.didReceiveMemoryWarning()
    }

    override func viewDidLoad() {
        super.viewDidLoad()
        toolsViewConstraint.constant = -self.toolsView.frame.width
        drawingSettingsContraint.constant = -self.drawingSettingsView.frame.width
        toolsView.layer.cornerRadius = 10
        drawingSettingsView.layer.cornerRadius = 10
        self.hideKeyboard()
        self.observeKeyboardNotification()
        if !connectionStatus {
            chatToggleBtn.isEnabled = false
        }
        if SocketManager.sharedInstance.delegate == nil {
            SocketManager.sharedInstance.delegate = self
        }

        drawingSettingsView.redField.delegate = self
        drawingSettingsView.greenField.delegate = self
        drawingSettingsView.blueField.delegate = self
        drawingSettingsView.alphaField.delegate = self
        drawingSettingsView.sizeField.delegate = self
    }

    override func viewDidAppear(_ animated: Bool) {
        super.viewDidAppear(animated)
    }

    @IBAction func editDrawingSettings(_ sender: Any) {
        drawingSettingsFn()
    }
    @IBAction func chatToggleBtn(_ sender: Any) {
        chatToggleFn()
    }

    @IBAction func toolsToggleBtn(_ sender: Any) {
        toolsToggleFn()
        if drawingSettingsShowing {
            drawingSettingsFn()
        }
    }

    func toolsToggleFn() {
        let sidebarWidth = self.toolsView.frame.width
        if toolsShowing {
            toolsViewConstraint.constant = -sidebarWidth
        } else {
            toolsViewConstraint.constant = 0
            toolsView.setDefault()
        }
        UIView.animate(withDuration: 0.3, animations: {self.view.layoutIfNeeded()})
        toolsShowing = !toolsShowing
    }

    func chatToggleFn() { //function called to toggle the chat view
        let windowWidth = self.drawView.frame.width
        let chatViewWidth = self.chatView.frame.width
        if chatShowing {
            chatViewConstraint.constant = windowWidth
        } else {
            chatViewConstraint.constant = windowWidth - chatViewWidth
        }
        UIView.animate(withDuration: 0.3, animations: {self.view.layoutIfNeeded()})
        self.chatToggleBtn.tintColor = UIColor.blue
        chatShowing = !chatShowing
    }

    func drawingSettingsFn() {
        let settingsView = self.drawingSettingsView.frame.width
        if drawingSettingsShowing {
            drawingSettingsContraint.constant = -settingsView
        } else {
            drawingSettingsContraint.constant = 0
        }
        UIView.animate(withDuration: 0.3, animations: {self.view.layoutIfNeeded()})
        drawingSettingsShowing = !drawingSettingsShowing
    }

    private func notifyMessage(message: String, messageInfos: (author: String, timestamp: String)) {
        self.timer?.invalidate()
        self.chatToggleBtn.tintColor = UIColor.red
        if self.chatShowing {
            self.timer = setTimeout(0.5) { //Flash red for 1 second if chat is showing
                self.chatToggleBtn.tintColor = UIColor.blue
            }
        }

        AudioServicesPlaySystemSound(SystemSoundID(1003)) //See https://github.com/TUNER88/iOSSystemSoundsLibrary
    }

    internal func connect() {
        print("Connecting to server.")
    }

    internal func disconnect(error: Error?) {
        print ("Disconnected with error: \(String(describing: error?.localizedDescription))")
    }

    internal func managerDidReceive(data: Data) {
        do {
            print("Data received.")
            let decoder = JSONDecoder()
            let incomingMessage = try decoder.decode(IncomingChatMessage.self, from: data)
            print(incomingMessage.message)
            let convertTime = Timestamp()
            let timestamp = convertTime.getTimeFromServer(timestamp: incomingMessage.timestamp)
            let messageInfos = (incomingMessage.author.username, timestamp)
            chatView.displayMessage(message: incomingMessage.message, messageInfos: messageInfos)
            if incomingMessage.author.username != AccountManager.sharedInstance.username {
                self.notifyMessage(message: incomingMessage.message, messageInfos: messageInfos)
            }
        } catch let error {
            print(error)
        }
    }

    func textField(_ textField: UITextField, shouldChangeCharactersIn range: NSRange, replacementString string: String) -> Bool {
        let allowedCharacters = CharacterSet.decimalDigits
        let characterSet = CharacterSet(charactersIn: string)

        if allowedCharacters.isSuperset(of: characterSet) {
            var maxTextFieldValue = 255

            if drawingSettingsView.redField.isEditing {
                maxTextFieldValue = 255
            }
            if drawingSettingsView.greenField.isEditing {
                maxTextFieldValue = 255
            }
            if drawingSettingsView.blueField.isEditing {
                maxTextFieldValue = 255
            }
            if drawingSettingsView.alphaField.isEditing {
                maxTextFieldValue = 100
            }
            if drawingSettingsView.sizeField.isEditing {
                maxTextFieldValue = 50
            }

            var startString = ""
            if (textField.text != nil) {
                startString += textField.text!
            }
            startString += string
            var limitNumber: Int = Int(startString)!
            if (limitNumber < 0) {
                return false
            }
            if limitNumber > maxTextFieldValue {
                if drawingSettingsView.redField.isEditing {
                   drawingSettingsView.redField.text! = String(maxTextFieldValue)
                }
                if drawingSettingsView.greenField.isEditing {
                    drawingSettingsView.greenField.text! = String(maxTextFieldValue)
                }
                if drawingSettingsView.blueField.isEditing {
                    drawingSettingsView.blueField.text! = String(maxTextFieldValue)
                }
                if drawingSettingsView.alphaField.isEditing {
                   drawingSettingsView.alphaField.text! = String(maxTextFieldValue)
                }
                if drawingSettingsView.sizeField.isEditing {
                    drawingSettingsView.sizeField.text! = String(maxTextFieldValue)
                }
                return false
            }
            else {
                return true
            }
        } else {
            return false
        }
    }
}
