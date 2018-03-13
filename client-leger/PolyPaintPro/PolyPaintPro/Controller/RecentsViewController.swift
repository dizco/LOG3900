//
//  RecentsViewController.swift
//  PolyPaintPro
//
//  Created by Fred Habsfan on 2018-03-07.
//  Copyright © 2018 Les Pods c'est pour les lunchs. All rights reserved.
//

import UIKit

class RecentsViewController: UIViewController {
  var connexionStatus = true

    @IBOutlet weak var newDrawingbutton: UIButton!
    @IBOutlet weak var openLocalDrawingButton: UIButton!
    @IBOutlet weak var joinDrawingButton: UIButton!
    @IBOutlet weak var joinGalleryButton: UIButton!
    @IBOutlet weak var backToLoginButton: UIButton!

    override func viewDidLoad() {
        super.viewDidLoad()
             // Do any additional setup after loading the view.
        print(connexionStatus)

        if (!connexionStatus) {
            joinDrawingButton.isEnabled = false
        } else if (connexionStatus) {
            backToLoginButton.isEnabled = false
            backToLoginButton.isHidden = true
        }
    }

    override func didReceiveMemoryWarning() {
        super.didReceiveMemoryWarning()
        // Dispose of any resources that can be recreated.
    }

}
