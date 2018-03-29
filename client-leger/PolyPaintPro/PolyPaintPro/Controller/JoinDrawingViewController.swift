//
//  JoinDrawingViewController.swift
//  PolyPaintPro
//
//  Created by Fred Habsfan on 2018-03-12.
//  Copyright © 2018 Les Pods c'est pour les lunchs. All rights reserved.
//

import Foundation
import UIKit
import PromiseKit

class JoinDrawingViewController: UIViewController {
    internal var connectionStatus = true
    var joinDrawingList: [OnlineDrawingModel] = []

    @IBOutlet weak var joinDrawingTableView: UITableView!

    override func viewDidLoad() {
        super.viewDidLoad()
        joinDrawingTableView.tableFooterView = UIView(frame: CGRect.zero)
        loadOnlineDrawings()
    }

    override func didReceiveMemoryWarning() {
        super.didReceiveMemoryWarning()
        // Dispose of any resources that can be recreated.
    }

    func insertNewDrawing(drawing: OnlineDrawingModel) {
        joinDrawingList.append(drawing)
        let indexPath = IndexPath(row: joinDrawingList.count - 1, section: 0)
        joinDrawingTableView.beginUpdates()
        joinDrawingTableView.insertRows(at: [indexPath], with: .automatic)
        joinDrawingTableView.endUpdates()
    }

    func tableView(_ tableView: UITableView, didSelectRowAt indexPath: IndexPath) {
        if (joinDrawingList[indexPath.row] as OnlineDrawingModel).protection.active { //protected drawing
            showAlert(indexPath: indexPath)
        } else { //not protected drawing
            if (joinDrawingList[indexPath.row] as OnlineDrawingModel).mode == DrawingTypes.Pixel {
                performSegue(withIdentifier: "JoinPixelDrawingSegue", sender: self)
            } else if (joinDrawingList[indexPath.row] as OnlineDrawingModel).mode == DrawingTypes.Stroke {
                performSegue(withIdentifier: "JoinStrokeDrawingSegue", sender: self)
            }
        }
    }

    func validatePassword(inputPassword: String) -> Bool {
        // MARK: - insert logic for pwd validation here
        return true
    }

    func showAlert(indexPath: IndexPath) {
        let alert = UIAlertController(title: "Image protégée",
                                      message: "Entrez le mot de passe pour accéder à l'image",
                                      preferredStyle: .alert)
        // Submit button
        let submitAction = UIAlertAction(title: "Submit", style: .default, handler: { _ -> Void in
            // Get 1st TextField's text
            let inputPassword = alert.textFields![0]
            print(inputPassword.text!)
            if self.validatePassword(inputPassword: inputPassword.text!) {
                if (self.joinDrawingList[indexPath.row] as OnlineDrawingModel).mode == DrawingTypes.Pixel {
                    self.performSegue(withIdentifier: "JoinPixelDrawingSegue", sender: self)
                } else if (self.joinDrawingList[indexPath.row] as OnlineDrawingModel).mode == DrawingTypes.Stroke {
                    self.performSegue(withIdentifier: "JoinStrokeDrawingSegue", sender: self)
                }
            } else if !self.validatePassword(inputPassword: inputPassword.text!) {
                inputPassword.text! = ""
            }
        })
        // Cancel button
        let cancel = UIAlertAction(title: "Cancel", style: .destructive, handler: { _ -> Void in })
        alert.addTextField { (textField: UITextField) in
            textField.keyboardAppearance = .dark
            textField.keyboardType = .default
            textField.autocorrectionType = .default
            textField.placeholder = "Mot de passe"
            textField.clearButtonMode = .whileEditing
            textField.isSecureTextEntry = true
        }
        // Add action buttons and present the Alert
        alert.addAction(submitAction)
        alert.addAction(cancel)
        present(alert, animated: true, completion: nil)
    }

    private func loadOnlineDrawings() {
        firstly {
            RestManager.getDrawingsListPage(page: 1)
        }.then { response -> Void in
            if response.success {
                for drawing in (response.data?.docs)! {
                    self.insertNewDrawing(drawing: drawing)
                }
                for index in 2...(response.data?.pages)! { //Load all other pages (other than page 1)
                    RestManager.getDrawingsListPage(page: index).then { response -> Void in
                        if response.success {
                            for drawing in (response.data?.docs)! {
                                self.insertNewDrawing(drawing: drawing)
                            }
                        } else {
                            print("Failed to get drawings page: \(index)")
                        }
                    }.catch { error in
                        print("Unexpected error during get drawings: \(error). At page: \(index)")
                    }
                }
            } else {
                print("Failed to get drawings page: 1")
            }
        }.catch { error in
            print("Unexpected error during get drawings: \(error). At page: 1")
        }
    }
}

extension JoinDrawingViewController: UITableViewDataSource, UITableViewDelegate {
    func tableView(_ tableView: UITableView, numberOfRowsInSection section: Int) -> Int {
        return joinDrawingList.count
    }

    func tableView(_ tableView: UITableView, cellForRowAt indexPath: IndexPath) -> UITableViewCell {
        let cellIdentifier = "Cell"
        let cell = tableView.dequeueReusableCell(withIdentifier: cellIdentifier, for: indexPath) as UITableViewCell

        cell.textLabel?.text = (joinDrawingList[indexPath.row] as OnlineDrawingModel).name
        if (joinDrawingList[indexPath.row] as OnlineDrawingModel).protection.active {
            cell.detailTextLabel?.text = "\u{1f512}"
        } else {
            cell.detailTextLabel?.text = "\u{1f513}"
        }
        return cell
    }
}
