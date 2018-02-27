import * as passport from "passport";
import * as passportLocal from "passport-local";
import { default as User } from "../models/User";
import { PassportVerifyLocal } from "./passport-verify-local";
import { NextFunction, Request, Response } from "express";

const LocalStrategy = passportLocal.Strategy;

passport.serializeUser<any, any>((user, done) => {
    done(undefined, user.id);
});

passport.deserializeUser((id, done) => {
    User.findById(id, (err, user) => {
        done(err, user);
    });
});


/**
 * Sign in using Email and Password.
 */
passport.use(new LocalStrategy({ usernameField: "email" }, PassportVerifyLocal.verify));

/**
 * Login Required middleware.
 */
export let isAuthenticated = (req: Request, res: Response, next: NextFunction) => {
    if (req.isAuthenticated()) {
        return next();
    }
    return res.status(401).json({ status: "error", error: "Unauthorized." });
};
