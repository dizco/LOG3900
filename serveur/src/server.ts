import * as errorHandler from "errorhandler";
import * as WebSocket from "ws";
import { IncomingMessage } from "http";
import { WebSocketDecorator } from "./decorators/websocket-decorator";
import { SocketMessage } from "./models/sockets/socket-message";
import { SocketStrategyContext } from "./strategies/sockets/socket-strategy-context";

const app = require("./app");

/**
 * Error Handler. Provides full stack - remove for production
 */
app.use(errorHandler());

/**
 * Start Express server.
 */
const server = app.listen(app.get("port"), () => {
  console.log(("  App is running at http://localhost:%d in %s mode"), app.get("port"), app.get("env"));
  console.log("  Press CTRL-C to stop\n");
});

const wss = new WebSocket.Server({ server });

wss.on("connection", (ws: WebSocket, req: IncomingMessage) => {
    const wsDecorator = new WebSocketDecorator(wss, ws);
    console.log("\nConnection by socket on server with id", req.connection.remoteAddress);

    ws.on("message", (message: any) => {
        const parsedMessage = <SocketMessage>JSON.parse(message);
        console.log("Message received", parsedMessage);

        //Interpret the message and determine the best strategy to use
        const strategyContext = new SocketStrategyContext(parsedMessage);
        strategyContext.execute(wsDecorator);
    });

    ws.on("error", (error) => {
        console.log("errored", error);
    });

    ws.on("close", (code, reason) => {
        console.log("disconnected from socket", code, reason);
    });

    wsDecorator.detectDisconnect();
});

export = server;