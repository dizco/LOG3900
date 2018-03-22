import * as WebSocket from "ws";
import { IncomingMessage } from "http";
import { WebSocketDecorator } from "./decorators/websocket-decorator";
import { SocketMessage } from "./models/sockets/socket-message";
import { SocketStrategyContext } from "./strategies/sockets/socket-strategy-context";
import { WebSocketServer } from "./websockets/websocket-server";
import { TryParseJSON } from "./helpers/json";
import { NextFunction, Request, Response } from "express";
import { app } from "./app";
import { verifyClient } from "./websockets/verify-client";
import { DefaultRooms } from "./websockets/default-rooms";
import { PassportVerifyLocal } from "./config/passport-verify-local";
import { setWss as drawingModelSetWss } from "./models/drawings/drawing";

/**
 * Error Handler. Provides full stack in dev and test
 */
app.use((err: any, req: Request, res: Response, next: NextFunction) => {
    console.log("Unhandled error detected.", err);
    if (req.app.get("env") === "development") {
        //Development env print stack trace
        return res.status(err.statusCode || 500).json({status: "error", error: err.message, stack: err.stack});
    }
    return res.status(err.statusCode || 500).json({status: "error", error: err.message});
});

/**
 * Start Express server.
 */
const server = app.listen(app.get("port"), () => {
    console.log(`  App is running at http://localhost:${app.get("port")} in ${app.get("env")} mode`);
    console.log("  Press CTRL-C to stop\n");
});

const wss = new WebSocketServer(server, verifyClient);
PassportVerifyLocal.setWss(wss);
drawingModelSetWss(wss);

wss.on("connection", (ws: WebSocket, req: IncomingMessage) => {
    const wsDecorator = new WebSocketDecorator(wss, ws);
    wsDecorator.user = (<any>req).identifiedUser;
    wsDecorator.join(DefaultRooms.General);
    wsDecorator.join(DefaultRooms.Chat);

    console.log(`\nConnection by socket on server with ip ${req.connection.remoteAddress}.\n`);

    //TODO: Add an event to allow users to subscribe to specific drawings

    ws.on("message", (message: any) => {
        const parsedMessage = TryParseJSON(message);
        if (!parsedMessage) {
            console.log("Impossible to parse message", message);
            return;
        }
        const socketMessage: SocketMessage = <SocketMessage>parsedMessage;
        if (process.env.NODE_ENV === "development") {
            console.log("Message received\n", socketMessage, "\n");
        }

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
        console.log(`\nDisconnected from socket ip ${req.connection.remoteAddress} with code ${code}. Reason : ${reason}.\n`);
        wss.removeClient(wsDecorator);
    });

    wsDecorator.detectDisconnect();
});

export { server };