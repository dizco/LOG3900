//
//  OpenLocalDrawingViewController.swift
//  PolyPaintPro
//
//  Created by Fred Habsfan on 2018-03-12.
//  Copyright © 2018 Les Pods c'est pour les lunchs. All rights reserved.
//

import Foundation
import UIKit

class OpenLocalDrawingViewController: UIViewController {
   internal var connectionStatus = true
    var drawingList: [LocalDrawingModel] = []

    @IBOutlet weak var openLocalDrawingTableView: UITableView!

    override func viewDidLoad() {
        super.viewDidLoad()
        openLocalDrawingTableView.tableFooterView = UIView(frame: CGRect.zero)
        drawingList.append(LocalDrawingModel(id: "123", name: "mona lisa", type: "trait")) //mocked data
        drawingList.append(LocalDrawingModel(id: "456", name: "msdkjfhx", type: "pixel")) //mocked data

        // Do any additional setup after loading the view.
    }

    override func didReceiveMemoryWarning() {
        super.didReceiveMemoryWarning()
        // Dispose of any resources that can be recreated.
    }

    func insertNewDrawing() {
        let indexPath = IndexPath(row: drawingList.count - 1, section: 0)
        //drawingList.append(OpenLocalDrawingsDataStruct(id: "456", name: "mona lisa", type: "pixel")) //mocked data this line adds an element to the table of drawings
        openLocalDrawingTableView.beginUpdates()
        openLocalDrawingTableView.insertRows(at: [indexPath], with: .automatic)
        openLocalDrawingTableView.endUpdates()
    }

    func tableView(_ tableView: UITableView, didSelectRowAt indexPath: IndexPath) {
        //function called when a table view cell is pressed
        if (drawingList[indexPath.row] as LocalDrawingModel).type == "pixel" {
            performSegue(withIdentifier: "OpenLocalPixelDrawingSegue", sender: self)
        } else if (drawingList[indexPath.row] as LocalDrawingModel).type == "trait" {
            performSegue(withIdentifier: "OpenLocalStrokeDrawingSegue", sender: self)
        }
    }
}

extension OpenLocalDrawingViewController: UITableViewDataSource, UITableViewDelegate {
    func tableView(_ tableView: UITableView, numberOfRowsInSection section: Int) -> Int {
        return drawingList.count
    }

    func tableView(_ tableView: UITableView, cellForRowAt indexPath: IndexPath) -> UITableViewCell {
        let cellIdentifier = "Cell"
        let cell = tableView.dequeueReusableCell(withIdentifier: cellIdentifier, for: indexPath) as UITableViewCell

        cell.textLabel?.text = (drawingList[indexPath.row] as LocalDrawingModel).name
        if (drawingList[indexPath.row] as LocalDrawingModel).type == "trait" {
            cell.detailTextLabel?.text = "Mode par trait"
        } else if (drawingList[indexPath.row] as LocalDrawingModel).type == "pixel" {
            cell.detailTextLabel?.text = "Mode par pixel"
        }

        return cell
    }
}
