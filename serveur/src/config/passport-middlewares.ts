import { NextFunction, Request, Response } from "express";
import * as _ from "lodash";

/**
 * Login Required middleware.
 */
export let isAuthenticated = (req: Request, res: Response, next: NextFunction) => {
    if (req.isAuthenticated()) {
        return next();
    }
    return res.status(401).json({ status: "error", error: "Unauthorized." });
};

/**
 * Authorization Required middleware.
 */
export let isAuthorized = (req: Request, res: Response, next: NextFunction) => {
    const provider = req.path.split("/").slice(-1)[0];

    if (_.find(req.user.tokens, { kind: provider })) {
        next();
    }
    else {
        res.redirect(`/auth/${provider}`);
    }
};