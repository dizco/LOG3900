import { Request, Response } from "express";

/**
 * GET /
 * Home page.
 */
export let index = (req: Request, res: Response) => {
    res.json({ "hey": "Hey" });
    /*res.render("home", {
      title: "Home"
    });*/
};
