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

    // MARK: - RestManager
    private var restManager: RestManager?
    internal var connectionStatus = true //variable for connexion status
    // MARK: - Labels
    @IBOutlet weak var welcomeLabel: UILabel!
    @IBOutlet weak var connectionErrorLabel: UILabel!

  // MARK: - Views
    @IBOutlet weak var scrollView: UIScrollView!
    @IBOutlet weak var connectionView: UIView?
    @IBOutlet weak var registerView: UIView?
    @IBOutlet weak var selectorView: UIView!
    @IBOutlet weak var serverInformationsView: UIView!

    // MARK: - Text fields
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

    // MARK: - Error messages labels
    @IBOutlet weak var loginErrorTextField: UILabel!
    @IBOutlet weak var registerErrorTextField: UILabel!

    override func viewDidAppear(_ animated: Bool) {
        super.viewDidAppear(animated)
    }

    override func didReceiveMemoryWarning() {
        super.didReceiveMemoryWarning()
    }

    override func viewDidLayoutSubviews() {
        scrollView.isScrollEnabled = true
        scrollView.contentSize = CGSize (width: scrollView.contentSize.width, height: 900)
        scrollView.contentOffset.y = 900 - 768
    }

    override func prepare(for segue: UIStoryboardSegue, sender: Any?) {
        let nav = segue.destination as? UINavigationController
        let secondController = nav!.topViewController as? RecentsViewController
        secondController!.connectionStatus = connectionStatus
    }

    override func viewDidLoad() {
        super.viewDidLoad()
        //registerView?.isHidden = true //default view is login
        //default values
        connectionView?.isHidden = true
        registerView?.isHidden = true
        selectorView?.isHidden = true
        connectionErrorLabel?.isHidden = true
        self.hideKeyboard()
        self.observeKeyboardNotification()
        loginErrorTextField?.isHidden = true
        registerErrorTextField?.isHidden = true
    }

    // MARK: - Buttons
    @IBAction func connexionButton(_ sender: UIButton) {
        let username = "me@me.ca"
        let password = "hahahaha"

        let usernameValidation = AccountManager.validateUsername(username: username)

        if usernameValidation.success {
            loginErrorTextField?.isHidden = true
            loginToServer(sender: sender, username: username, password: password)
        } else {
            loginErrorTextField?.text = usernameValidation.error
            loginErrorTextField?.isHidden = false
        }
    }

    @IBAction func offlineMode(_ sender: UIButton) {
        connectionStatus = false
        performSegue(withIdentifier: "offline", sender: self)
    }

    @IBAction func registerButton(_ sender: UIButton) {
        let username = registerUsernameField!.text!
        let password = registerPasswordField!.text!

        let registerValidation = AccountManager.validateRegister(username: username, password: password)

        if registerValidation.success {
            registerErrorTextField?.isHidden = true
            registerAccount(sender: sender, username: username, password: password)
        } else {
            registerErrorTextField?.text = registerValidation.error
            registerErrorTextField?.isHidden = false
        }
    }

    @IBAction func loginToggle(_ sender: UISegmentedControl) {
        if sender.selectedSegmentIndex == 0 {
            welcomeLabel.text = "Bienvenue! Entrez vos informations de connexion PolyPaintPro"
            connectionView?.isHidden = false
            registerView?.isHidden = true
        } else if sender.selectedSegmentIndex == 1 {
            welcomeLabel.text = "Bienvenue! Entrez vos informations pour créer un compte PolyPaintPro"
            connectionView?.isHidden = true
            registerView?.isHidden = false
        }
    }

    @IBAction func serverAddressEnteredButton(_ sender: UIButton) {
        //attempt to connect to the server modify the connectionState and errorMessage
        serverAddressField!.text = "192.168.50.9"
        let isTrueIP = ServerLookup.sharedInstance.saveServerAddress(withIPAddress: serverAddressField!.text!)
        serverAddressEntered(connectionState: isTrueIP)
    }

    // MARK: - Functions
    private func registerAccount(sender: UIButton, username: String, password: String) {
        print("try to register")
        firstly {
            RestManager.registerToServer(username: username, password: password)
        }.then { response -> Void in
            if response.success {
                self.loginErrorTextField?.isHidden = true
                // Account creation successful: auto login immediately
                self.loginToServer(sender: sender, username: username, password: password)
            } else {
                self.registerErrorTextField?.text = response.error
                self.registerErrorTextField?.isHidden = false
            }
        }.catch { error in
            print("Unexpected error during register: \(error)")
        }
    }

    private func loginToServer(sender: UIButton, username: String, password: String) {
        print("try to login")
        firstly {
            RestManager.loginToServer(username: username, password: password)
        }.then { response -> Void in
            if response.success {
                self.loginErrorTextField?.isHidden = true
                AccountManager.sharedInstance.saveUser(userId: response.data!.id, username: username)
                SocketManager.sharedInstance.establishConnection(ipAddress: ServerLookup.sharedInstance.address)
                self.performSegue(withIdentifier: "welcome", sender: sender)
            } else {
                self.loginErrorTextField?.text = response.error
                self.loginErrorTextField?.isHidden = false
            }
        }.catch { error in
            print("Unexpected error during login: \(error)")
        }
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
}
