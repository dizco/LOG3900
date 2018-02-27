import { VerifyFunction } from "passport-local";
import { default as User, UserModel } from "../models/User";
import { WebSocketServer } from "../websockets/websocket-server";

/**
 * Static instance of the "verify" function to supply to the Local strategy of authentication
 * Aims to solve a circular dependency with the usage of the WebSocketServer
 */
export abstract class PassportVerifyLocal {
    private static wss: WebSocketServer;

    public static setWss(server: WebSocketServer) {
        this.wss = server;
    }

    public static verify: VerifyFunction = (email, password, done) => {
        if (!PassportVerifyLocal.wss) {
            return done(new Error("WebSocketServer must be initialized before verifying a user."));
        }
        User.findOne({ username: email.toLowerCase() }, (err, user: any) => {
            if (err) {
                return done(err);
            }
            if (!user) {
                return done(undefined, false, { message: `Email ${email} not found.` });
            }

            user = <UserModel>user;
            user.comparePassword(password, (err: Error, isMatch: boolean) => {
                if (err) {
                    return done(err);
                }
                if (isMatch) {
                    if (PassportVerifyLocal.isAlreadyLoggedIn(user)) {
                        return done(undefined, false, { message: "User is already logged in." });
                    }
                    return done(undefined, user);
                }
                return done(undefined, false, { message: "Invalid email or password." });
            });
        });
    };

    private static isAlreadyLoggedIn(user: UserModel): boolean {
        const exists = this.wss.userExists(user);
        if (exists) {
            console.log(`Attempted to login with username ${user.username}, but another client is still connected with it.`);
        }
        return exists;
    }
}