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
    var drawingListName : [String] = ["John", "Adam", "Shilpa", "Jennifer", "Sia"] //array containing the list of drawing
    var drawingPrivacyStatus: [Bool] = [true, true, false, false, true] //array containing the privacy status of each drawing

    @IBOutlet weak var joinDrawingTableView: UITableView!

    override func viewDidLoad() {
        super.viewDidLoad()
        //joinDrawingTableView.tableFooterView = UIView(frame: CGRect.zero)
    }
    
    override func didReceiveMemoryWarning() {
        super.didReceiveMemoryWarning()
        // Dispose of any resources that can be recreated.
    }

    func insertNewDrawing() {
        let indexPath = IndexPath(row: drawingListName.count - 1 , section: 0)
        //drawingListName.append(" ciboire de saint osti") this line adds an element to the table of drawings
        //drawingPrivacyStatus.append(true) thi line adds the privacy status of the drawing to the table
        joinDrawingTableView.beginUpdates()
        joinDrawingTableView.insertRows(at: [indexPath], with: .automatic)
        joinDrawingTableView.endUpdates()
    }
}

extension JoinDrawingViewController: UITableViewDataSource, UITableViewDelegate {
    func tableView(_ tableView: UITableView, numberOfRowsInSection section: Int) -> Int {
        return drawingListName.count
    }

    func tableView(_ tableView: UITableView, cellForRowAt indexPath: IndexPath) -> UITableViewCell {
        let cellIdentifier = "Cell"
        let cell = tableView.dequeueReusableCell(withIdentifier: cellIdentifier, for: indexPath) as UITableViewCell

        cell.textLabel?.text = drawingListName[indexPath.row]
        if drawingPrivacyStatus[indexPath.row] {
            cell.detailTextLabel?.text = "\u{1f513}"
        } else if !drawingPrivacyStatus[indexPath.row] {
            cell.detailTextLabel?.text = "\u{1f512}"
        }
        return cell
    }
}
