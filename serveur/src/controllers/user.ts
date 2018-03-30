import * as passport from "passport";
import { default as User, UserModel } from "../models/User";
import { NextFunction, Request, Response } from "express";
import { IVerifyOptions } from "passport-local";
import { WriteError } from "mongodb";

const enum LoginFields {
    Username = "email",
    Password = "password",
}

/**
 * POST /login
 * Sign in using email and password.
 */
export let postLogin = (req: Request, res: Response, next: NextFunction) => {
    req.checkBody(LoginFields.Username, "Email is not valid").isEmail();
    req.checkBody(LoginFields.Password, "Password cannot be blank").notEmpty();
    req.sanitizeBody(LoginFields.Username).normalizeEmail({gmail_remove_dots: false});

    const errors = req.validationErrors();

    if (errors) {
        return res.status(422).json({ status: "error", error: "Validation errors.", hints: errors });
    }

    passport.authenticate("local", (err: Error, user: UserModel, info: IVerifyOptions) => {
        if (err) {
            return next(err);
        }
        if (!user) {
            return res.status(401).json({ status: "error", error: info.message });
        }
        req.logIn(user, (err) => {
            if (err) {
                return next(err);
            }
            return res.json({ status: "success", objectId: user.id });
        });
    })(req, res, next);
};

/**
 * POST /logout
 * Log out.
 */
export let logout = (req: Request, res: Response) => {
    req.logout();
    return res.json({status: "success"});
};

/**
 * POST /register
 * Create a new local account.
 */
export let postRegister = (req: Request, res: Response, next: NextFunction) => {
    req.checkBody(LoginFields.Username, "Email is not valid").isEmail();
    req.checkBody(LoginFields.Password, "Password must be at least 8 characters long").len({ min: 8 });
    req.sanitize(LoginFields.Username).normalizeEmail({ gmail_remove_dots: false });

    const errors = req.validationErrors();

    if (errors) {
        return res.status(422).json({status: "error", error: "Validation errors.", hints: errors});
    }

    const user = new User({
        username: req.body[LoginFields.Username],
        password: req.body[LoginFields.Password]
    });

    User.findOne({ username: req.body[LoginFields.Username] }, (err, existingUser) => {
        if (err) {
            return next(err);
        }
        if (existingUser) {
            //409 status code might not be the MOST appropriate statusCode, because it might instruct consumers that they can try again
            return res.status(409).json({ status: "error", error: "Account with that email address already exists." });
        }
        user.save((err) => {
            if (err) {
                return next(err);
            }
            req.logIn(user, (err) => {
                if (err) {
                    return next(err);
                }
                return res.json({ status: "success", objectId: user.id });
            });
        });
    });
};

/**
 * POST /account/password
 * Update current password.
 */
/*export let postUpdatePassword = (req: Request, res: Response, next: NextFunction) => {
    req.assert("password", "Password must be at least 4 characters long").len({ min: 4 });
    req.assert("confirmPassword", "Passwords do not match").equals(req.body.password);

    const errors = req.validationErrors();

    if (errors) {
        req.flash("errors", errors);
        return res.redirect("/account");
    }

    User.findById(req.user.id, (err, user: UserModel) => {
        if (err) {
            return next(err);
        }
        user.password = req.body.password;
        user.save((err: WriteError) => {
            if (err) {
                return next(err);
            }
            req.flash("success", { msg: "Password has been changed." });
            res.redirect("/account");
        });
    });
};*/

/**
 * POST /account/delete
 * Delete user account.
 */
/*export let postDeleteAccount = (req: Request, res: Response, next: NextFunction) => {
    User.remove({ _id: req.user.id }, (err) => {
        if (err) {
            return next(err);
        }
        req.logout();
        req.flash("info", { msg: "Your account has been deleted." });
        res.redirect("/");
    });
};*/
