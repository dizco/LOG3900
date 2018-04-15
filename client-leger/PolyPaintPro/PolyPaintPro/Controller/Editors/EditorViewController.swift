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

class EditorViewController: UIViewController, ChatSocketManagerDelegate, iCarouselDataSource, iCarouselDelegate {
    private var colorsValidator: TextFieldValidator!
    private var alphaValidator: TextFieldValidator!
    private var sizeValidator: TextFieldValidator!
    private var timer: Timer!
    internal var chatShowing = false
    internal var toolsShowing = false
    internal var drawingSettingsShowing = false
    internal var connectionStatus = true
    internal var drawing: IncomingDrawing?

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
    @IBOutlet var tutorialCarousel: iCarousel!
    var strokeTutorialImages = [String]()
    var pixelTutorialImages = [String]()

    override func didReceiveMemoryWarning() {
        super.didReceiveMemoryWarning()
    }

    override func viewDidLoad() {
        super.viewDidLoad()
          print(drawing?.mode)
        tutorialCarousel.type = .coverFlow2
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
        self.subscribeToSocketActions()

        if drawing?.mode == "stroke" {
            if let showTutorial = UserDefaults.standard.object(forKey: "strokeTutorialStatus") {
                endTutorial()
            }
        } else {
            if let showTutorial = UserDefaults.standard.object(forKey: "pixelTutorialStatus") {
                endTutorial()
            }
        }
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
        print(self.drawView.frame.width)
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
            if !AccountManager.sharedInstance.isMyself(id: incomingMessage.author.id) {
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

    internal func unsubscribeFromSocketActions() {
        if SocketManager.sharedInstance.getConnectionStatus() {
            do {
                let outgoingSubscription = SubscriptionMessage(actionId: "leave", actionName: "",
                                                               drawingId: (self.drawing?.id)!)
                let encodedData = try JSONEncoder().encode(outgoingSubscription)
                SocketManager.sharedInstance.send(data: encodedData)
            } catch let error {
                print(error)
            }
        }
    }

    private func subscribeToSocketActions() {
        if SocketManager.sharedInstance.getConnectionStatus() {
            do {
                let outgoingSubscription = SubscriptionMessage(actionId: "join", actionName: "",
                                                               drawingId: (self.drawing?.id)!)
                let encodedData = try JSONEncoder().encode(outgoingSubscription)
                SocketManager.sharedInstance.send(data: encodedData)
            } catch let error {
                print(error)
            }
        }
    }

    func numberOfItems(in carousel: iCarousel) -> Int {
        if drawing?.mode == "stroke" {
            return strokeTutorialImages.count
        } else {
            return pixelTutorialImages.count
        }

    } 

    func carousel(_ carousel: iCarousel, viewForItemAt index: Int, reusing view: UIView?) -> UIView {
        let tempView = UIView(frame: CGRect(x: 0, y: 0, width: 784, height: 628))
        tempView.layer.cornerRadius = 10
          var tutorialImages = UIImageView(frame: CGRect(x: 0, y: 0, width: 784, height: 588))
        if drawing?.mode == "stroke" {
            tutorialImages.image = UIImage(named: strokeTutorialImages[index])!
        } else {
            tutorialImages.image = UIImage(named: pixelTutorialImages[index])!
        }

        tutorialImages.layer.cornerRadius = 10
        tutorialImages.clipsToBounds = true

        let nextButton = UIButton(frame: CGRect(x: 714, y: 588, width: 70, height: 40))
        nextButton.tag = index
        nextButton.layer.cornerRadius = 10
        nextButton.backgroundColor = UIColor.blue
        nextButton.setTitle("Suivant", for: .normal)
        nextButton.setTitleColor(UIColor.black, for: .normal)
        nextButton.addTarget(self, action: #selector(nextTutorialSlide), for: .touchUpInside)

        let previousButton = UIButton(frame: CGRect(x: 0, y: 588, width: 95, height: 40))
        previousButton.tag = index
        previousButton.layer.cornerRadius = 10
        previousButton.backgroundColor = UIColor.blue
        previousButton.setTitle("Précédent", for: .normal)
        previousButton.setTitleColor(UIColor.black, for: .normal)
        previousButton.addTarget(self, action: #selector(previousTutorialSlide), for: .touchUpInside)

        let endTutorialButton = UIButton(frame: CGRect(x: 607, y: 588, width: 87, height: 40))
        endTutorialButton.layer.cornerRadius = 10
        endTutorialButton.backgroundColor = UIColor.blue
        endTutorialButton.setTitle("Terminer", for: .normal)
        endTutorialButton.setTitleColor(UIColor.black, for: .normal)
        endTutorialButton.addTarget(self, action: #selector(endTutorial), for: .touchUpInside)

        tempView.addSubview(tutorialImages)
        tempView.addSubview(endTutorialButton)
        tempView.addSubview(nextButton)
        tempView.addSubview(previousButton)
        return tempView
    }

    func carousel(_ carousel: iCarousel, valueFor option: iCarouselOption, withDefault value: CGFloat) -> CGFloat {
        if option == iCarouselOption.spacing {
            return value * 1.2
        }
        return value
    }

    override func awakeFromNib() {
        super.awakeFromNib()
        strokeTutorialImages = ["1welcomeImage", "2strokeChatTutorial", "3strokePenTutorial", "4strokeShowDrawingSettingsTutorial", "5strokeDrawingToolsTutorial", "6strokeEraserTutorial", "7strokeSegmentEraserTutorial", "8strokeResetTutorial", "9strokeStackTutorial", "10strokeUnstackTutorial", "11strokeShowTutorialTutorial", "12strokeExitTutorial"]
        pixelTutorialImages = ["1welcomeImage", "2pixelChatTutorial", "3pixelPenTutorial", "4pixelShowDrawingSettingsTutorial", "5pixelDrawingToolsTutorial", "5pixelSegmentEraserTutorial", "6pixelShowTutorialTutorial", "7pixelExitTutorial"]
    }

    @objc func nextTutorialSlide (sender: UIButton) {
        print("button pressed")
        print(sender.tag)
        if drawing?.mode == "stroke" {
            if sender.tag == strokeTutorialImages.count - 1 {
                endTutorial()
                UserDefaults.standard.set(true, forKey: "strokeTutorialStatus")
            } else {
                tutorialCarousel.scrollToItem(at: sender.tag + 1, animated: true)
            }
        } else {

            if sender.tag == pixelTutorialImages.count - 1 {
                endTutorial()
                UserDefaults.standard.set(true, forKey: "pixelTutorialStatus")
            } else {
                tutorialCarousel.scrollToItem(at: sender.tag + 1, animated: true)
            }
        }
    }
    @objc func previousTutorialSlide (sender: UIButton) {
            tutorialCarousel.scrollToItem(at: sender.tag - 1, animated: true)
    }

    @objc func endTutorial () {
        print("tutorial ended")
        tutorialCarousel.isHidden = true
    }

    @IBAction func showTutorialButton(_ sender: UIButton) {
        if toolsShowing {
            toolsToggleFn()
        }
        if drawingSettingsShowing {
            drawingSettingsFn()
        }
        tutorialCarousel.scrollToItem(at:0, animated: true)
        tutorialCarousel.isHidden = false

    }

}
