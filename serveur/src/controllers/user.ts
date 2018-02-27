//import * as async from "async";
//import * as crypto from "crypto";
//import * as nodemailer from "nodemailer";
import * as passport from "passport";
import { default as User, UserModel } from "../models/User";
import { NextFunction, Request, Response } from "express";
import { IVerifyOptions } from "passport-local";
//import { WriteError } from "mongodb";

//const request = require("express-validator");

const enum LoginFields {
    Username = "email",
    Password = "password",
}

/**
 * GET /login
 * Login page.
 */
/*export let getLogin = (req: Request, res: Response) => {
    if (req.user) {
        return res.redirect("/");
    }
    res.render("account/login", {
        title: "Login"
    });
};*/

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
        return res.status(422).json({status: "error", error: "Validation errors.", hints: errors});
    }

    passport.authenticate("local", (err: Error, user: UserModel, info: IVerifyOptions) => {
        if (err) {
            return next(err);
        }
        if (!user) {
            return res.status(401).json({status: "error", error: info.message});
        }
        req.logIn(user, (err) => {
            if (err) {
                return next(err);
            }
            //res.redirect(req.session.returnTo || "/");
            return res.json({status: "success"});
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
 * GET /signup
 * Signup page.
 */
/*export let getSignup = (req: Request, res: Response) => {
    if (req.user) {
        return res.redirect("/");
    }
    res.render("account/signup", {
        title: "Create Account"
    });
};*/

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
        email: req.body[LoginFields.Username],
        password: req.body[LoginFields.Password]
    });

    User.findOne({ email: req.body[LoginFields.Username] }, (err, existingUser) => {
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
                return res.json({status: "success"});
            });
        });
    });
};

/**
 * GET /account
 * Profile page.
 */
/*export let getAccount = (req: Request, res: Response) => {
    res.render("account/profile", {
        title: "Account Management"
    });
};*/

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

/**
 * GET /forgot
 * Forgot Password page.
 */
/*export let getForgot = (req: Request, res: Response) => {
    if (req.isAuthenticated()) {
        return res.redirect("/");
    }
    res.render("account/forgot", {
        title: "Forgot Password"
    });
};*/

/**
 * POST /forgot
 * Create a random token, then the send user an email with a reset link.
 */
/*export let postForgot = (req: Request, res: Response, next: NextFunction) => {
    req.assert("email", "Please enter a valid email address.").isEmail();
    req.sanitize("email").normalizeEmail({ gmail_remove_dots: false });

    const errors = req.validationErrors();

    if (errors) {
        req.flash("errors", errors);
        return res.redirect("/forgot");
    }

    async.waterfall([
        function createRandomToken(done: Function) {
            crypto.randomBytes(16, (err, buf) => {
                const token = buf.toString("hex");
                done(err, token);
            });
        },
        function setRandomToken(token: AuthToken, done: Function) {
            User.findOne({ email: req.body.email }, (err, user: any) => {
                if (err) {
                    return done(err);
                }
                if (!user) {
                    req.flash("errors", { msg: "Account with that email address does not exist." });
                    return res.redirect("/forgot");
                }
                user.passwordResetToken = token;
                user.passwordResetExpires = Date.now() + 3600000; // 1 hour
                user.save((err: WriteError) => {
                    done(err, token, user);
                });
            });
        },
        function sendForgotPasswordEmail(token: AuthToken, user: UserModel, done: Function) {
            const transporter = nodemailer.createTransport({
                service: "SendGrid",
                auth: {
                    user: process.env.SENDGRID_USER,
                    pass: process.env.SENDGRID_PASSWORD
                }
            });
            const mailOptions = {
                to: user.email,
                from: "hackathon@starter.com",
                subject: "Reset your password on Hackathon Starter",
                text: `You are receiving this email because you (or someone else) have requested the reset of the password for your account.\n\n
          Please click on the following link, or paste this into your browser to complete the process:\n\n
          http://${req.headers.host}/reset/${token}\n\n
          If you did not request this, please ignore this email and your password will remain unchanged.\n`
            };
            transporter.sendMail(mailOptions, (err) => {
                req.flash("info", { msg: `An e-mail has been sent to ${user.email} with further instructions.` });
                done(err);
            });
        }
    ], (err) => {
        if (err) {
            return next(err);
        }
        res.redirect("/forgot");
    });
};*/
