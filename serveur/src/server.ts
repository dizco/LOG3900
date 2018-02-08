import * as WebSocket from "ws";
import { IncomingMessage } from "http";
import { WebSocketDecorator } from "./decorators/websocket-decorator";
import { SocketMessage } from "./models/sockets/socket-message";
import { SocketStrategyContext } from "./strategies/sockets/socket-strategy-context";
import { WebSocketServer } from "./websockets/websocket-server";
import { TryParseJSON } from "./helpers/json";
import { NextFunction, Request, Response } from "express";
import { app, mongoStore /*, sessionParser*/ } from "./app";
import { VerifyClientCallbackAsync, VerifyClientCallbackSync } from "ws";
import { default as User } from "./models/User";
const cookie = require("cookie");


/**
 * Error Handler. Provides full stack in dev and test
 */
app.use((err: any, req: Request, res: Response, next: NextFunction) => {
    console.log("Unhandled error detected.", err);
    if (req.app.get("env") === "development" || req.app.get("env") === "test") {
        //Development and test environments print stack traces
        return res.status(err.statusCode || 500).json({status: "error", error: err.message, stack: err.stack});
    }
    return res.status(err.statusCode || 500).json({status: "error", error: err.message});
});

/**
 * Start Express server.
 */
const server = app.listen(app.get("port"), () => {
    console.log(("  App is running at http://localhost:%d in %s mode"), app.get("port"), app.get("env"));
    console.log("  Press CTRL-C to stop\n");
});

let identifiedUser: any = undefined; //TODO: NOT store the user like this...

const verifyClient: VerifyClientCallbackSync | VerifyClientCallbackAsync = (info, done) => {
    const tS = cookie.parse((<any>info.req).headers.cookie)["connect.sid"];
    const sessionID = tS.split(".")[0].substr(4); //TODO: Identify why the cookie has 4 extra characters at the beginning...
    mongoStore.get(sessionID, function(err, session) {
        User.findById(session.passport.user, (err, user) => {
            identifiedUser = user;
            if (user === undefined) {
                console.log("Could not verify the client using sessions. WebSockets will not be accessible.");
            }
            done(user !== undefined);
        });
    });

    //TODO: Remove following comments if sessions work out fine
    /*sessionParser(<any>info.req, <any>{}, () => {
        console.log("Session is parsed!", (<any>info.req).user);

        // We can reject the connection by returning false to done(). For example,
        // reject here if user is unknown.
        done((<any>info.req).user);
    });*/
};

const wss = new WebSocketServer(server, verifyClient);

wss.on("connection", (ws: WebSocket, req: IncomingMessage) => {
    const wsDecorator = new WebSocketDecorator(wss, ws);
    wss.join("test", wsDecorator); //TODO: Remove

    console.log("\nConnection by socket on server with ip", req.connection.remoteAddress);

    ws.on("message", (message: any) => {
        wsDecorator.user = identifiedUser;
        const parsedMessage = TryParseJSON(message);
        if (!parsedMessage) {
            console.log("Impossible to parse message", message);
            return;
        }
        const socketMessage: SocketMessage = <SocketMessage>parsedMessage;
        console.log("Message received", socketMessage);

        //Interpret the message and determine the best strategy to use
        if (SocketStrategyContext.canParse(socketMessage)) {
            const strategyContext = new SocketStrategyContext(socketMessage);
            strategyContext.execute(wsDecorator);
        }
        else {
            console.log("Impossible to identify a socket message.");
        }
    });

    ws.on("error", (error) => {
        console.log(`Error on WebSocket with ip ${req.connection.remoteAddress} : ${error.message}`);
    });

    ws.on("close", (code, reason) => {
        console.log(`Disconnected from socket ip ${req.connection.remoteAddress} with code ${code}. Reason : ${reason}.`);
        wss.remove(wsDecorator);
    });

    wsDecorator.detectDisconnect();
});

export = server;