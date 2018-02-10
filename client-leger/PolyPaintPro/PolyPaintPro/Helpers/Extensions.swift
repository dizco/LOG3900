//
//  Extensions.swift
//  PolyPaintPro
//
//  Created by Fred Habsfan on 2018-02-03.
//  Copyright © 2018 Les Pods c'est pour les lunchs. All rights reserved.
//

import Foundation
import UIKit
//extension used in conjunction with self.hideKeyboard() in view DidLoad
//used to hide the chat keyboard when the user taps somewhere else than on the Kb
extension UIViewController {
    func hideKeyboard() {
        let tap: UITapGestureRecognizer = UITapGestureRecognizer(
            target: self,
            action: #selector(UIViewController.dismissKeyboard))
        view.addGestureRecognizer(tap)    }

    @objc func dismissKeyboard() {
        view.endEditing(true)
    }
}

extension ChatView: UITableViewDataSource, UITableViewDelegate {
    func tableView(_ tableView: UITableView, numberOfRowsInSection section: Int) -> Int {
        return titleHeading.count
    }
    func tableView(_ tableView: UITableView, cellForRowAt indexPath: IndexPath) -> UITableViewCell {
        let cellIdentifier = "Cell"
        let cell = tableView.dequeueReusableCell(withIdentifier: cellIdentifier, for: indexPath) as UITableViewCell
        cell.textLabel?.attributedText = authorNameMutableString
        cell.textLabel?.text = titleHeading[indexPath.row]
        cell.detailTextLabel?.text = subtitleHeading[indexPath.row]
        cell.detailTextLabel?.numberOfLines = 0
        cell.textLabel?.numberOfLines = 0
        //chatTableView.rowHeight = UITableViewAutomaticDimension
        return cell
    }
}
