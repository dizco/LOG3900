import { VerifyClientCallbackAsync, VerifyClientCallbackSync } from "ws";
import { mongoStore } from "../app";
import { default as User } from "../models/User";
const cookie = require("cookie");

export const verifyClient: VerifyClientCallbackSync | VerifyClientCallbackAsync = (info, done) => {
    let connectSid: string = "";
    try {
        connectSid = cookie.parse((<any>info.req).headers.cookie)["connect.sid"];
        connectSid = decodeURIComponent(connectSid);
    }
    catch (e) {
        //Error parsing the cookie
        console.log("Could not parse cookies on request. WebSockets will not be accessible.");
        done(false);
        return;
    }
    const sessionID = connectSid.split(".")[0].split(":")[1];
    mongoStore.get(sessionID, function(err, session) {
        if (err) {
            console.log("Could not extract the session from database. WebSockets will not be accessible.");
            done(false);
            return;
        }
        if (!("passport" in session && "user" in session.passport)) {
            console.log("Could not extract the user from the session. WebSockets will not be accessible.");
            done(false);
            return;
        }
        User.findById(session.passport.user, (err, user) => {
            (<any>info.req).identifiedUser = user;
            if (user === undefined) {
                console.log("Could not verify the client using sessions. WebSockets will not be accessible.");
            }
            done(user !== undefined);
        });
    });
};