//
//  JoinGalleryViewController.swift
//  PolyPaintPro
//
//  Created by Fred Habsfan on 2018-03-12.
//  Copyright © 2018 Les Pods c'est pour les lunchs. All rights reserved.
//

import Foundation
import UIKit

class JoinGalleryViewController: UIViewController {
    @IBOutlet var webView: UIWebView!

    override func viewDidLoad() {
        super.viewDidLoad()
        let url = NSURL (string: "https://www.google.ca/")
        let request = NSURLRequest(url: url! as URL)
        webView.loadRequest(request as URLRequest)
    }

    override func didReceiveMemoryWarning() {
        super.didReceiveMemoryWarning()
        // Dispose of any resources that can be recreated.
    }
}
