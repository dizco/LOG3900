import * as WebSocket from "ws";
import * as http from "http";
import * as https from "https";
import { ServerOptions } from "ws";
import { WebSocketDecorator } from "../decorators/websocket-decorator";
import { Room } from "./room";

/*
Extends the default WebSocket.Server to add Rooms support
 */
export class WebSocketServer extends WebSocket.Server {
    private rooms: Room[];

    public constructor(server: http.Server | https.Server) {
        const options: ServerOptions = {
            server: server
        };
        super(options);

        this.rooms = [];
    }

    public findRoom(id: string): Room {
        return this.rooms.find((room: Room) => {
            return room.getId() === id;
        });
    }

    public join(id: string, client: WebSocketDecorator): void {
        let room = this.findRoom(id);
        if (!room) {
            room = new Room(id);
            this.rooms.push(room);
        }

        room.addClient(client);
    }

    public remove(client: WebSocketDecorator): void {
        this.rooms.forEach((room: Room) => {
            room.removeClient(client);
        });
        this.rooms = this.rooms.filter((room: Room) => {
            return !room.isEmpty();
        });
    }

}