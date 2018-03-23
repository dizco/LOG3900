import { WebSocketDecorator } from "../decorators/websocket-decorator";
import * as WebSocket from "ws";
import { Poll } from "../models/sockets/poll";

export class Room {
    protected static readonly POLL_INTERVAL = 10000; //10 seconds

    private id: string;
    private clients: WebSocketDecorator[];
    private isPolling: boolean;
    private pollingInterval: NodeJS.Timer;

    public constructor(id: string) {
        this.id = id;
        this.clients = [];
        this.isPolling = false;
    }

    public getId(): string {
        return this.id;
    }

    public getClients(): WebSocketDecorator[] {
        return this.clients;
    }

    public addClient(client: WebSocketDecorator): void {
        if (this.clients.indexOf(client) === -1) {
            this.clients.push(client);
        }
    }

    public removeClient(clientToRemove: WebSocketDecorator): void {
        this.clients = this.clients.filter((client: WebSocketDecorator) => {
            return clientToRemove !== client;
        });
    }

    public isEmpty(): boolean {
        return this.clients.length === 0;
    }

    /**
     * Send a message to every other client in the room
     * @param data
     * @param {WebSocketDecorator} ws
     */
    public broadcast(data: any, ws: WebSocketDecorator): boolean {
        this.clients.forEach((client: WebSocketDecorator) => {
            if (client.getWs().readyState === WebSocket.OPEN) {
                client.getWs().send(data); //TODO: Return false if send fails
            }
        });
        return true;
    }

    public startPolling(): void {
        if (this.isPolling)
            return;

        this.isPolling = true;
        this.pollingInterval = setInterval(this.poll, Room.POLL_INTERVAL);
    }

    public stopPolling(): void {
        if (!this.isPolling)
            return;

        this.isPolling = false;
        clearInterval(this.pollingInterval);
    }

    private poll = () => { //Maintain 'this' on the object, and not on the interval
        const index = Math.floor(Math.random() * this.clients.length);
        this.clients[index].getWs().send(JSON.stringify(this.buildPollMessage()));
    };

    private buildPollMessage(): Poll {
        return {
            type: "server.editor.poll",
            drawing: {
                id: this.id,
            },
            timestamp: Date.now(),
        };
    }
}

