//
//  RecentsViewController.swift
//  PolyPaintPro
//
//  Created by Fred Habsfan on 2018-03-07.
//  Copyright © 2018 Les Pods c'est pour les lunchs. All rights reserved.
//

import UIKit

class RecentsViewController: UIViewController {
    var connectionStatus = true

    @IBOutlet weak var newDrawingbutton: UIButton!
    @IBOutlet weak var openLocalDrawingButton: UIButton!
    @IBOutlet weak var joinDrawingButton: UIButton!
    @IBOutlet weak var joinGalleryButton: UIButton!
    @IBOutlet weak var backToLoginButton: UIButton!

    override func viewDidLoad() {
        super.viewDidLoad()
        if !connectionStatus {
            joinDrawingButton.isEnabled = false
        } else {
            backToLoginButton.isEnabled = false
            backToLoginButton.isHidden = true
        }
    }
    @IBAction func newDrawingButton(_ sender: UIButton) {
        performSegue(withIdentifier: "NewDrawingSegue", sender: self)
    }

    override func prepare(for segue: UIStoryboardSegue, sender: Any?) {
        let vc = segue.destination as! NewDrawingViewController
        vc.connectionStatus = connectionStatus
    }

    override func didReceiveMemoryWarning() {
        super.didReceiveMemoryWarning()
        // Dispose of any resources that can be recreated.
    }

}
