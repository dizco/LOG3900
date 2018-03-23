//
//  JoinDrawingViewController.swift
//  PolyPaintPro
//
//  Created by Fred Habsfan on 2018-03-12.
//  Copyright © 2018 Les Pods c'est pour les lunchs. All rights reserved.
//

import Foundation
import UIKit

class JoinDrawingViewController: UIViewController {
    internal var connectionStatus = true
    var joinDrawingList: [JoinDrawingsDataStruct] = []

    @IBOutlet weak var joinDrawingTableView: UITableView!

    override func viewDidLoad() {
        super.viewDidLoad()
        joinDrawingTableView.tableFooterView = UIView(frame: CGRect.zero)
        joinDrawingList.append(JoinDrawingsDataStruct(id: "123", name: "mona lisa", privacyStatus: true, type: "trait")) //mocked data
        joinDrawingList.append(JoinDrawingsDataStruct(id: "456", name: "msdkjfhx", privacyStatus: false, type: "pixel")) //mocked data
    }
    
    override func didReceiveMemoryWarning() {
        super.didReceiveMemoryWarning()
        // Dispose of any resources that can be recreated.
    }

    func insertNewDrawing() {
        let indexPath = IndexPath(row: joinDrawingList.count - 1, section: 0)
        //drawingList.append(OpenLocalDrawingsDataStruct(id: "456", name: "mona lisa", type: "pixel")) //mocked data this line adds an element to the table of drawings
        joinDrawingTableView.beginUpdates()
        joinDrawingTableView.insertRows(at: [indexPath], with: .automatic)
        joinDrawingTableView.endUpdates()
    }

    func tableView(_ tableView: UITableView, didSelectRowAt indexPath: IndexPath) {
        if (joinDrawingList[indexPath.row] as JoinDrawingsDataStruct).privacyStatus { //protected drawing
            let alert = UIAlertController(title: "AlertController Tutorial",
                                          message: "Submit something",
                                          preferredStyle: .alert)
            // Submit button
            let submitAction = UIAlertAction(title: "Submit", style: .default, handler: { (action) -> Void in
                // Get 1st TextField's text
                let inputPwd = alert.textFields![0]
                print(inputPwd.text!)
                if self.validatePwd(inputPwd: inputPwd.text!) {
                    if (self.joinDrawingList[indexPath.row] as JoinDrawingsDataStruct).type == "pixel" {
                        self.performSegue(withIdentifier: "JoinPixelDrawingSegue", sender: self)
                    } else if (self.joinDrawingList[indexPath.row] as JoinDrawingsDataStruct).type == "trait" {
                        self.performSegue(withIdentifier: "JoinStrokeDrawingSegue", sender: self)
                    }
                } else if !self.validatePwd(inputPwd: inputPwd.text!) {
                    inputPwd.text! = ""
                }
            })
            // Cancel button
            let cancel = UIAlertAction(title: "Cancel", style: .destructive, handler: { (action) -> Void in })
            alert.addTextField { (textField: UITextField) in
                textField.keyboardAppearance = .dark
                textField.keyboardType = .default
                textField.autocorrectionType = .default
                textField.placeholder = "Type something here"
                textField.clearButtonMode = .whileEditing
            }
            // Add action buttons and present the Alert
            alert.addAction(submitAction)
            alert.addAction(cancel)
            present(alert, animated: true, completion: nil)

        } else if !(joinDrawingList[indexPath.row] as JoinDrawingsDataStruct).privacyStatus { //not protected drawing
            if (joinDrawingList[indexPath.row] as JoinDrawingsDataStruct).type == "pixel" {
                performSegue(withIdentifier: "JoinPixelDrawingSegue", sender: self)
            } else if (joinDrawingList[indexPath.row] as JoinDrawingsDataStruct).type == "trait" {
                performSegue(withIdentifier: "JoinStrokeDrawingSegue", sender: self)
            }
        }
    }

    func validatePwd(inputPwd: String) -> Bool {
        // MARK: - insert logic for pwd validation here
        return true
    }
}

extension JoinDrawingViewController: UITableViewDataSource, UITableViewDelegate {
    func tableView(_ tableView: UITableView, numberOfRowsInSection section: Int) -> Int {
        return joinDrawingList.count
    }

    func tableView(_ tableView: UITableView, cellForRowAt indexPath: IndexPath) -> UITableViewCell {
        let cellIdentifier = "Cell"
        let cell = tableView.dequeueReusableCell(withIdentifier: cellIdentifier, for: indexPath) as UITableViewCell

        cell.textLabel?.text = (joinDrawingList[indexPath.row] as JoinDrawingsDataStruct).name
        if (joinDrawingList[indexPath.row] as JoinDrawingsDataStruct).privacyStatus {
            cell.detailTextLabel?.text = "\u{1f512}"
        } else if !(joinDrawingList[indexPath.row] as JoinDrawingsDataStruct).privacyStatus {
            cell.detailTextLabel?.text = "\u{1f513}"
        }
        return cell
    }
}
