import * as errorHandler from "errorhandler";
import * as SocketIO from "socket.io";

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
    socket.join("123");
    console.log("Connection by socket on chat with id", socket.conn.id);

    socket.emit("server.chat.message", {
        "message": "Bonjour à tous!",
        "room": {
            "id": 199,
            "name": "Main Chat"
        },
        "author": {
            "id": 134,
            "username": "dizco",
            "name": "Gabriel",
            "url": "https://example.com/users/dizco",
            "avatar_url": "https://example.com/users/dizco/avatar.jpg"
        }
    });

    socket.on("disconnect", () => {
        console.log("disconnected from chat");
    });
});

const editor = io.of("/editor").on("connection", (socket: SocketIO.Socket) => {
    socket.join("123");
    console.log("Connection by socket on editor with id", socket.conn.id);

    editor.emit("server.editor.action", {
        "action": {
            "id": 3,
            "name": "Fill"
        },
        "drawing": {
            "id": 199,
            "name": "Mona Lisa",
            "owner": {
                "id": 132,
                "username": "fred",
                "name": "Frédéric",
                "url": "https://example.com/users/fred",
                "avatar_url": "https://example.com/users/fred/avatar.jpg"
            }
        },
        "author": {
            "id": 134,
            "username": "dizco",
            "name": "Gabriel",
            "url": "https://example.com/users/dizco",
            "avatar_url": "https://example.com/users/dizco/avatar.jpg"
        }
    });

    editor.to("123").emit("server.editor.action", {"hello": "yes"});

    socket.on("disconnect", () => {
        console.log("disconnected from editor");
    });
});

export = server;