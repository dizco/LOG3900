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
        if (joinDrawingList[indexPath.row] as JoinDrawingsDataStruct).type == "pixel" {
            performSegue(withIdentifier: "JoinPixelDrawingSegue", sender: self)
        } else if (joinDrawingList[indexPath.row] as JoinDrawingsDataStruct).type == "trait" {
            performSegue(withIdentifier: "JoinStrokeDrawingSegue", sender: self)
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

        cell.textLabel?.text = (joinDrawingList[indexPath.row] as JoinDrawingsDataStruct).name
        if (joinDrawingList[indexPath.row] as JoinDrawingsDataStruct).privacyStatus {
            cell.detailTextLabel?.text = "\u{1f513}"
        } else if !(joinDrawingList[indexPath.row] as JoinDrawingsDataStruct).privacyStatus {
            cell.detailTextLabel?.text = "\u{1f512}"
        }
        return cell
    }
}
