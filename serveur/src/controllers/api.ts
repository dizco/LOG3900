"use strict";

//import * as graph from "fbgraph";
//import { NextFunction, Request, Response } from "express";


/**
 * GET /api
 * List of API examples.
 */
/*export let getApi = (req: Request, res: Response) => {
    res.render("api/index", {
        title: "API Examples"
    });
};*/

/**
 * GET /api/facebook
 * Facebook API example.
 */
/*export let getFacebook = (req: Request, res: Response, next: NextFunction) => {
    const token = req.user.tokens.find((token: any) => token.kind === "facebook");
    graph.setAccessToken(token.accessToken);
    graph.get(`${req.user.facebook}?fields=id,name,email,first_name,last_name,gender,link,locale,timezone`, (err: Error, results: graph.FacebookUser) => {
        if (err) {
            return next(err);
        }
        res.render("api/facebook", {
            title: "Facebook API",
            profile: results
        });
    });
};*/
