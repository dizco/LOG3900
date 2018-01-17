import * as errorHandler from "errorhandler";
import * as SocketIO from "socket.io";
import { ClientChatMessage } from "./models/sockets/client-chat-message";
import { ServerChatMessage } from "./models/sockets/server-chat-message";
import { ChatMessageFactory } from "./factories/chat-message-factory";
import { EditorActionFactory } from "./factories/editor-action-factory";
import { ClientEditorAction } from "./models/sockets/client-editor-action";

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

const io: SocketIO.Server = SocketIO(server);
const chat = io.of("/chat").on("connection", (socket: SocketIO.Socket) => {
    socket.join("123"); //TODO: Manage actual rooms, each drawing should be associated with a room
    console.log("Connection by socket on chat with id", socket.conn.id);

    socket.on("client.chat.message", (message: ClientChatMessage) => {
        console.log("got client message", message.message);
        const serverMessage = ChatMessageFactory.CreateServerChatMessage(socket, message);
        socket.broadcast.to("123").emit("server.chat.message", serverMessage);
    });

    socket.on("disconnect", () => {
        console.log("disconnected from chat");
    });
});

const editor = io.of("/editor").on("connection", (socket: SocketIO.Socket) => {
    socket.join("123"); //TODO: Manage actual rooms, each drawing should be associated with a room
    console.log("Connection by socket on editor with id", socket.conn.id);

    socket.on("client.editor.action", (action: ClientEditorAction) => {
        console.log("got client action", action);
        const serverAction = EditorActionFactory.CreateServerEditorAction(socket, action);
        socket.broadcast.to("123").emit("server.editor.action", serverAction);
    });

    socket.on("disconnect", () => {
        console.log("disconnected from editor");
    });
});

export = server;