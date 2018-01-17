import * as WebSocket from "ws";
import { clearInterval } from "timers";

export class WebSocketDecorator {
    protected readonly PING_INTERVAL = 60000; //60 seconds

    private wss: WebSocket.Server;
    private ws: WebSocket;
    private detectingDisconnects: boolean;
    private disconnectsInterval: NodeJS.Timer;
    private isAlive: boolean;

    public constructor(wss: WebSocket.Server, ws: WebSocket) {
        this.wss = wss;
        this.ws = ws;
        this.detectingDisconnects = false;
    }

    public getWs(): WebSocket {
        return this.ws;
    }

    /**
     * Send a message to every other client on the server
     * @param data
     */
    public broadcast(data: any): void {
        this.wss.clients.forEach((client: any) => {
            if (client !== this.ws && client.readyState === WebSocket.OPEN) {
                client.send(data);
            }
        });
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
        this.ws.ping(); //Ping client
    }
}
