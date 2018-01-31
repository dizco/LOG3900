import { WebSocketDecorator } from "../decorators/websocket-decorator";
import * as WebSocket from "ws";

export class Room {
    private id: string;
    private clients: WebSocketDecorator[];

    public constructor(id: string) {
        this.id = id;
        this.clients = [];
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
            if (client !== ws && client.getWs().readyState === WebSocket.OPEN) {
                client.getWs().send(data);
            }
        });
        return true;
    }
}
