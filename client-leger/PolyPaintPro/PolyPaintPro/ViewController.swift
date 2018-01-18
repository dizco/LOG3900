//
//  ViewController.swift
//  PolyPaintPro
//
//  Created by Fred Habsfan on 2018-01-16.
//  Copyright © 2018 Frederic. All rights reserved.
//

import UIKit

class ViewController: UIViewController {
    
    @IBOutlet weak var landScapeConstraint: NSLayoutConstraint!
  
    @IBOutlet weak var portrateConstraint: NSLayoutConstraint!
    
    
    
    func viewWillTransitionToSize(size: CGSize,   withTransitionCoordinator coordinator:    UIViewControllerTransitionCoordinator) {
        
        coordinator.animate(alongsideTransition: { (UIViewControllerTransitionCoordinatorContext) -> Void in
            
            let orient = UIApplication.shared.statusBarOrientation
            
            switch orient {
            case .portrait:
                print("Portrait")
                self.ApplyportraitConstraint()
                break
            // Do something
            default:
                print("LandScape")
                // Do something else
                self.applyLandScapeConstraint()
                break
            }
        }, completion: { (UIViewControllerTransitionCoordinatorContext) -> Void in
            print("rotation completed")
        })
        viewWillTransitionToSize(size: size, withTransitionCoordinator: coordinator)
    }
    
    
    
    
    
    
    
    
    
    
    
    func ApplyportraitConstraint(){
       
        self.view.addConstraint(self.portrateConstraint)
        self.view.removeConstraint(self.landScapeConstraint)
    }
    func applyLandScapeConstraint(){
        
        self.view.removeConstraint(self.portrateConstraint)
        self.view.addConstraint(self.landScapeConstraint)
    }

    override func viewDidLoad() {
        super.viewDidLoad()
       
        // Do any additional setup after loading the view, typically from a nib.
    }

    override func didReceiveMemoryWarning() {
        super.didReceiveMemoryWarning()
        // Dispose of any resources that can be recreated.
    }
    
    
   

}

