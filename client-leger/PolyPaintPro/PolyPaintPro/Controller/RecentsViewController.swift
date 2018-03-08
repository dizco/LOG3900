//
//  RecentsViewController.swift
//  PolyPaintPro
//
//  Created by Fred Habsfan on 2018-03-07.
//  Copyright © 2018 Les Pods c'est pour les lunchs. All rights reserved.
//

import UIKit

class RecentsViewController: UIViewController {
  var status = true

    override func viewDidLoad() {
        super.viewDidLoad()
             // Do any additional setup after loading the view.
    }

    override func didReceiveMemoryWarning() {
        super.didReceiveMemoryWarning()
        // Dispose of any resources that can be recreated.
    }

    @IBAction func newDrawing(_ sender: UIButton) {
        performSegue(withIdentifier: "create", sender: self)
    }

    @IBAction func openDrawing(_ sender: UIButton) {
        performSegue(withIdentifier: "open", sender: self)
    }

    override func prepare(for segue: UIStoryboardSegue, sender: Any?) {
        let vc = segue.destination as! ViewController
        vc.status = status
    }

}
