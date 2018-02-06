import * as errorHandler from "errorhandler";
import * as WebSocket from "ws";
import { IncomingMessage } from "http";
import { WebSocketDecorator } from "./decorators/websocket-decorator";
import { SocketMessage } from "./models/sockets/socket-message";
import { SocketStrategyContext } from "./strategies/sockets/socket-strategy-context";
import { WebSocketServer } from "./websockets/websocket-server";
import { TryParseJSON } from "./helpers/json";
import { NextFunction, Request, Response } from "express";

const app = require("./app");

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

const wss = new WebSocketServer(server);

wss.on("connection", (ws: WebSocket, req: IncomingMessage) => {
    const wsDecorator = new WebSocketDecorator(wss, ws);
    wss.join("test", wsDecorator); //TODO: Remove

    //TODO: Use the user in the session to insert him inside relevant rooms

    console.log("\nConnection by socket on server with ip", req.connection.remoteAddress);

    ws.on("message", (message: any) => {
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