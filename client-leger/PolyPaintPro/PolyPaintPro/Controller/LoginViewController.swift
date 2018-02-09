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
    @IBOutlet weak var registerNameField: UITextField!
    @IBOutlet weak var registerFirstNameField: UITextField!
    @IBOutlet weak var registerPasswordField: UITextField!
    @IBOutlet weak var registerPasswordValidationField: UITextField!
    //error messages text field
    @IBOutlet weak var loginErrorTextField: UILabel!
    @IBOutlet weak var registerErrorTextField: UILabel!
    //Buttons
    @IBAction func connexionButton(_ sender: UIButton) {
        if AccountManager.sharedInstance.validateUsername(username: loginUsernameField!.text!) {
            loginErrorTextField?.isHidden = true
            loginToServer(sender: sender)
        } else {
            loginErrorTextField?.text = AccountManager.sharedInstance.usernameError
            loginErrorTextField?.isHidden = false
        }
    }

    private func loginToServer(sender: UIButton) {
        print("try to login")
        let loginManager = Login(username: loginUsernameField!.text!, password: loginPasswordField!.text!)
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

    @IBAction func registerButton(_ sender: Any) {
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
        //expecting 2 return values, a boolean for connectionState and a string for the error message, if there is an error
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
