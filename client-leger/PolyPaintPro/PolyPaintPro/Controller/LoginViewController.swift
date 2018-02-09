//
//  LoginViewController.swift
//  PolyPaintPro
//
//  Created by Fred Habsfan on 2018-02-08.
//  Copyright © 2018 Les Pods c'est pour les lunchs. All rights reserved.
//
import UIKit
import Alamofire
import PromiseKit

class LoginViewController: UIViewController {
    //Labels
    @IBOutlet weak var welcomeLabel: UILabel!
    @IBOutlet weak var connectionErrorLabel: UILabel!
    //views
    @IBOutlet var placeHolderView: UIView!
    @IBOutlet weak var connectionView: UIView?
    @IBOutlet weak var registerView: UIView?
    @IBOutlet weak var selectorView: UIView!
    @IBOutlet weak var serverInformationsView: UIView!
    //text fields
    //server address textfield
    @IBOutlet weak var serverAddressField: UITextField!
    //login text fields
    @IBOutlet weak var loginUsernameField: UITextField!
    @IBOutlet weak var loginPasswordField: UITextField!
    //register view text fields
    @IBOutlet weak var registerUsernameField: UITextField!
    @IBOutlet weak var registerNameField: UITextField! // @fred: "uninstalled from the view" not visible anymore
    @IBOutlet weak var registerFirstNameField: UITextField! // @fred: "uninstalled from the view" not visible anymore
    @IBOutlet weak var registerPasswordField: UITextField!
    @IBOutlet weak var registerPasswordValidationField: UITextField! // @fred: "uninstalled from the view" not visible anymore
    //error messages labels
    @IBOutlet weak var loginErrorTextField: UILabel!
    @IBOutlet weak var registerErrorTextField: UILabel! //@fred: connected to view
    //Buttons
    @IBAction func connexionButton(_ sender: UIButton) {
        let username = loginUsernameField!.text!
        let password = loginPasswordField!.text!
        if AccountManager.sharedInstance.validateUsername(username: username) {
            loginErrorTextField?.isHidden = true
            loginToServer(sender: sender, username: username, password: password)
        } else {
            loginErrorTextField?.text = AccountManager.sharedInstance.usernameError
            loginErrorTextField?.isHidden = false
        }
    }

    private func loginToServer(sender: UIButton, username: String, password: String) {
        print("try to login")
        let loginManager = Login(username: username, password: password)
        firstly {
            loginManager.connectToServer()
        }.then { response -> Void in
            if response == true {
                self.loginErrorTextField?.isHidden = true
                // TO-MOVE: Connect with socket only in ChatViewController
                // TO-DO: Establish connection ONLY after the LOGIN POST
                SocketManager.sharedInstance.establishConnection(ipAddress: ServerLookup.sharedInstance.address)
                self.performSegue(withIdentifier: "welcome", sender: sender)
            } else {
                self.loginErrorTextField?.text = "Votre courriel et/ou votre mot de passe est invalide."
                self.loginErrorTextField?.isHidden = false
            }
        }.catch { error in
                print(error)
        }
    }

    @IBAction func registerButton(_ sender: UIButton) {
        // need a registration error button
        let username = registerUsernameField!.text!
        let password = registerPasswordField!.text!
        if AccountManager.sharedInstance.validateRegister(username: username, password: password) {
            registerErrorTextField?.isHidden = true
            registerAccount(sender: sender, username: username, password: password)
        } else {
            registerErrorTextField?.text = AccountManager.sharedInstance.registerError
            registerErrorTextField?.isHidden = false
        }
    }

    private func registerAccount(sender: UIButton, username: String, password: String) {
        print("try to register")
        let registerManager = Register(username: username, password: password)
        firstly {
            registerManager.connectToServer()
            }.then { response -> Void in
                if response == true {
                    self.loginErrorTextField?.isHidden = true
                    // Account creation successful: auto login immediately
                    self.loginToServer(sender: sender, username: username, password: password)
                } else {
                    self.registerErrorTextField?.text = "Une erreur inconnue est survenue lors de l'enregistrement."
                    self.registerErrorTextField?.isHidden = false
                }
            }.catch { error in
                print(error)
        }
    }

    @IBAction func loginToggle(_ sender: UISegmentedControl) {
        if sender.selectedSegmentIndex == 0 {
            welcomeLabel.text = "Bienvenue! Entrez vos informations de connexion PolyPaintPro"
            connectionView?.isHidden = false
            registerView?.isHidden = true
        } else if sender.selectedSegmentIndex == 1 {
            welcomeLabel.text = "Bienvenue! Entrez vos informations pour creer un compte PolyPaintPro"
            connectionView?.isHidden = true
            registerView?.isHidden = false
        }
    }
    @IBAction func serverAddressEnteredButton(_ sender: UIButton) {
        //attempt function to attempt to connect to the server modify the connectionState and errorMessage
        let isTrueIP = ServerLookup.sharedInstance.saveServerAddress(withIPAddress: serverAddressField!.text!)
        serverAddressEntered(connectionState: isTrueIP)
    }
    func serverAddressEntered(connectionState: Bool) {
        if connectionState { //connection with the server established
            connectionView?.isHidden = false
            selectorView?.isHidden = false
            serverInformationsView?.isHidden = true
        } else { //error when trying to connect to the server
            connectionErrorLabel?.isHidden = false
            connectionErrorLabel?.text = ServerLookup.sharedInstance.error
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
        connectionView?.isHidden = true
        registerView?.isHidden = true
        selectorView?.isHidden = true
        connectionErrorLabel?.isHidden = true
        self.hideKeyboard()
        observeKeyboardNotification()
        loginErrorTextField?.isHidden = true
        registerErrorTextField?.isHidden = true
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
}
