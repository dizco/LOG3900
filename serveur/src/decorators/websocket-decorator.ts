import * as WebSocket from "ws";
import { clearInterval } from "timers";
import { WebSocketServer } from "../websockets/websocket-server";
import { UserModel } from "../models/User";

export class WebSocketDecorator {
    //Here we unfortunately can't extend WebSocket directly, because the WebSocket instance is created inside the WebSocket.Server

    protected readonly PING_INTERVAL = 60000; //60 seconds

    private wss: WebSocketServer;
    private ws: WebSocket;
    private detectingDisconnects: boolean;
    private disconnectsInterval: NodeJS.Timer;
    private isAlive: boolean;

    public user: UserModel;

    public constructor(wss: WebSocketServer, ws: WebSocket) {
        this.wss = wss;
        this.ws = ws;
        this.detectingDisconnects = false;
    }

    public getWs(): WebSocket {
        return this.ws;
    }

    /**
     * Allows to broadcast to all or only to a room. Imitates Socket.IO structure
     * https://stackoverflow.com/a/10099325/6316091
     * Use like :
     * wsDecorator.broadcast.send("hello");
     * wsDecorator.broadcast.to("123").send("hello");
     */
    public broadcast = {
        send: (data: any) => this.broadcastToAll(data),
        to: (roomId: string) => {
            //Select a room to broadcast to
            //Return an object with a function "send"
            return {
                send: (data: any) => this.broadcastToRoom(data, roomId), //Insert the roomId selected previously in the function call
            };
        },
    };

    /**
     * Send a message to every other client on the server
     * @param data
     */
    private broadcastToAll(data: any): boolean {
        this.wss.clients.forEach((client: any) => {
            if (client.readyState === WebSocket.OPEN) {
                client.send(data); //TODO: Return false if send fails

            }
        });
        return true;
    }

    /**
     * Send a message to every other client in the room
     * @param data
     * @param {string} roomId
     */
    private broadcastToRoom(data: any, roomId: string): boolean {
        const room = this.wss.findRoom(roomId);
        if (room) {
            return room.broadcast(data, this);
        }
        return false;
    }

    /**
     * Join a WebSocket room
     * @param {string} roomId
     */
    public join(roomId: string): void {
        this.wss.joinRoom(roomId, this);
    }

    /**
     * Leave a WebSocket room
     * @param {string} roomId
     */
    public leave(roomId: string): void {
        this.wss.leaveRoom(roomId, this);
    }

    /**
     * Sends a ping on the socket with an interval and checks if the pong was received
     */
    public detectDisconnect(): void {
        if (this.detectingDisconnects)
            return;

        this.detectingDisconnects = true;
        this.ws.on("pong", () => this.isAlive = true); //The spec guarantees that any client will send a pong if it receives a ping

        this.disconnectsInterval = setInterval(this.ping, this.PING_INTERVAL);
    }

    private ping = () => { //Maintain 'this' on the object, and not on the interval
        if (this.isAlive === false) {
            this.detectingDisconnects = false;
            clearInterval(this.disconnectsInterval);
            return this.ws.terminate();
        }

        this.isAlive = false; //Set to false until we receive the pong reply

        if (this.ws.readyState === WebSocket.OPEN) {
            this.ws.ping(); //Ping client
        }
    }
}
