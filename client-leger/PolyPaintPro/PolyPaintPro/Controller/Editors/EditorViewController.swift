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

class EditorViewController: UIViewController, ChatSocketManagerDelegate, EditorViewControllerDelegate {
    private var colorsValidator: TextFieldValidator!
    private var alphaValidator: TextFieldValidator!
    private var sizeValidator: TextFieldValidator!
    private var timer: Timer!
    internal var chatShowing = false
    var toolsShowing = false
    var drawingSettingsShowing = false
    internal var connectionStatus = true

    let pixel = PixelEditorViewController()

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
        toolsView.editorDelegate = self
        toolsViewConstraint.constant = -self.toolsView.frame.width
        drawingSettingsContraint.constant = -self.drawingSettingsView.frame.width
        toolsView.layer.cornerRadius = 10
        drawingSettingsView.layer.cornerRadius = 10
        self.hideKeyboard()
        self.observeKeyboardNotification()
        if !connectionStatus {
            chatToggleBtn.isEnabled = false
        }
        if SocketManager.sharedInstance.chatDelegate == nil {
            SocketManager.sharedInstance.chatDelegate = self
        }

        let leftSwipe = UIScreenEdgePanGestureRecognizer(target: self, action: #selector(leftEdgeSwiped))
        leftSwipe.edges = .left
        view.addGestureRecognizer(leftSwipe)

        let rightSwipe = UIScreenEdgePanGestureRecognizer(target: self, action: #selector(rightEdgeSwiped))
        rightSwipe.edges = .right
        view.addGestureRecognizer(rightSwipe)

        self.initializeTextFieldValidators()
    }

    override func viewDidAppear(_ animated: Bool) {
        super.viewDidAppear(animated)
    }

    func getToolsShowing() -> Bool {
        return toolsShowing
    }

    func getDrawingSettingsShowing() -> Bool {
        return drawingSettingsShowing
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
        pixel.toggleFiltersToolsView()
    }

    func toggleFiltersToolsView () {

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
            self.timer = setTimeout(0.5) { //Flash red for 0.5 seconds if chat is showing
                self.chatToggleBtn.tintColor = UIColor.blue
            }
        }

        AudioServicesPlaySystemSound(SystemSoundID(1003)) //See https://github.com/TUNER88/iOSSystemSoundsLibrary
    }

    // MARK: - ChatSocketManagerDelegate
    internal func connect() {
        print("Connecting to server.")
    }

    internal func disconnect(error: Error?) {
        print ("Disconnected with error: \(String(describing: error?.localizedDescription))")
    }

    internal func managerDidReceiveChat(data: Data) {
        do {
            print("Chat data received.")
            let decoder = JSONDecoder()
            let incomingMessage = try decoder.decode(IncomingChatMessage.self, from: data)
            print(incomingMessage.message)
            let convertTime = Timestamp()
            let timestamp = convertTime.getTimeFromServer(timestamp: incomingMessage.timestamp)
            let messageInfos = (incomingMessage.author.username, timestamp)
            chatView.displayMessage(message: incomingMessage.message, messageInfos: messageInfos)
            if AccountManager.sharedInstance.isMyself(id: incomingMessage.author.id) {
                self.notifyMessage(message: incomingMessage.message, messageInfos: messageInfos)
            }
        } catch let error {
            print(error)
        }


    }

    // MARK: - Text field validators
    private func initializeTextFieldValidators() {
        self.colorsValidator = TextFieldValidator(minValue: 0, maxValue: 255)
        self.alphaValidator = TextFieldValidator(minValue: 0, maxValue: 100)
        self.sizeValidator = TextFieldValidator(minValue: 0, maxValue: 50)
        drawingSettingsView.redField.delegate = self.colorsValidator
        drawingSettingsView.greenField.delegate = self.colorsValidator
        drawingSettingsView.blueField.delegate = self.colorsValidator
        drawingSettingsView.alphaField.delegate = self.alphaValidator
        drawingSettingsView.sizeField.delegate = self.sizeValidator
    }

    // MARK: - Gestures
    @objc func leftEdgeSwiped(_ recognizer: UIScreenEdgePanGestureRecognizer) {
        if recognizer.state == .recognized {
            if (!toolsShowing && !drawingSettingsShowing) || (toolsShowing && drawingSettingsShowing) {
                toolsToggleFn()
                drawingSettingsFn()
            } else if toolsShowing && !drawingSettingsShowing {
                drawingSettingsFn()
            }
        }
    }

    @objc func rightEdgeSwiped(_ recognizer: UIScreenEdgePanGestureRecognizer) {
        if recognizer.state == .recognized {
            if connectionStatus {
                chatToggleFn()
            }
        }
    }
}
