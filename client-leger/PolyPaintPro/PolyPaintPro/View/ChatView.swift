//
//  ChatView.swift
//  PolyPaintPro
//
//  Created by Fred Habsfan on 2018-02-03.
//  Copyright © 2018 Les Pods c'est pour les lunchs. All rights reserved.
//
import Starscream
import UIKit

class ChatView: UIView, SocketManagerDelegate {
    internal var rowNumber: Int = 0
    internal var titleHeading: [String] = [""]
    internal var subtitleHeading: [String] = [""]
    internal var authorNameMutableString = NSMutableAttributedString()
    @IBOutlet weak var chatTableView: UITableView!
    @IBOutlet weak var messageField: UITextField!

    @IBAction func sendButton(_ sender: UIButton) {
        sendMessage()
    }

    func sendMessage() {
        let receivedMessage = messageField!.text
        let whitespaceSet = CharacterSet.whitespaces

        if !receivedMessage!.trimmingCharacters(in: whitespaceSet).isEmpty {
            do {
                let outgoingMessage = OutgoingChatMessage(message: receivedMessage!)
                let encodedData = try JSONEncoder().encode(outgoingMessage)
                SocketManager.sharedInstance.send(data: encodedData)
            } catch let error {
                print(error)
            }
        }
    }

    override init(frame: CGRect) {
        super.init(frame: frame)
        chatTableView.estimatedRowHeight = 55
        chatTableView.rowHeight = UITableViewAutomaticDimension
    }

    required init?(coder aDecoder: NSCoder) {
        super.init(coder: aDecoder)
        if SocketManager.sharedInstance.delegate == nil {
            SocketManager.sharedInstance.delegate = self
        }
    }

    func displayMessage(message: String, messageInfos: (author: String, timestamp: String)) {
        let indexPath = IndexPath.init(row: rowNumber, section: 0)
        let messageInfo = messageInfos.author + " " + messageInfos.timestamp
        authorNameMutableString = NSMutableAttributedString(
            string: messageInfo,
            attributes: [NSAttributedStringKey.font: UIFont.systemFont(ofSize: 13)]
        )
        authorNameMutableString.addAttribute(
            NSAttributedStringKey.font,
            value: UIFont.boldSystemFont(ofSize: 15),
            range: NSRange(location: 0, length: messageInfos.author.count)
        )
        titleHeading.insert(messageInfo, at: rowNumber)
        subtitleHeading.insert(message, at: rowNumber)
        updateContentInsetForTableView(tableView: chatTableView, animated: true)
        chatTableView.insertRows(at: [indexPath], with: .right)
        rowNumber += 1
        messageField.text = ""
    }

    func updateContentInsetForTableView(tableView: UITableView, animated: Bool) {
        let lastRow = tableView.numberOfRows(inSection: 0)
        let lastIndex = lastRow > 0 ? lastRow - 1 : 0
        let lastIndexPath = IndexPath(row: lastIndex, section: 9)
        let lastCellFrame = tableView.rectForRow(at: lastIndexPath)
        let topInset = max(tableView.frame.height - lastCellFrame.origin.y - lastCellFrame.height, 0)
        var contentInset = tableView.contentInset
        contentInset.top = topInset
        _ = UIViewAnimationOptions.beginFromCurrentState
        UIView.animate(withDuration: 0.1, animations: { () -> Void in
            tableView.contentInset = contentInset
        }
        )
    }

    func connect() {
        print("Connecting to server.")
    }

    func disconnect(error: Error?) {
        print ("Disconnected with error: \(String(describing: error?.localizedDescription))")
    }

    func managerDidReceive(data: Data) {
        do {
            print("Data received.")
            let decoder = JSONDecoder()
            let incomingMessage = try decoder.decode(IncomingChatMessage.self, from: data)
            print(incomingMessage.message)
            let convertTime = Timestamp()
            let timestamp = convertTime.getTimeFromServer(timestamp: incomingMessage.timestamp)
            let messageInfos = (incomingMessage.author.username, timestamp)
            self.displayMessage(message: incomingMessage.message, messageInfos: messageInfos)
        } catch let error {
            print(error)
        }
    }
}
