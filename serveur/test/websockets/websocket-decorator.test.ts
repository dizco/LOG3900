import { WebSocketDecorator } from "../../src/decorators/websocket-decorator";
import { expect } from "chai";
import * as sinon from "sinon";
import { SinonSandbox } from "sinon";
import { FakeWebSocket } from "./fake-websocket";
import { WebSocketServer } from "../../src/websockets/websocket-server";
import * as http from "http";
import { FakeWebSocketServer } from "./fake-websocket-server";

describe("websocket decorator", function() {
    describe("creation", function() {
        let sandbox: SinonSandbox;

        beforeEach(function() {
            sandbox = sinon.sandbox.create();
        });

        afterEach(function() {
            sandbox.restore();
        });

        it("should create a new websocket decorator", function() {
            const ws1 = new FakeWebSocket("ws://localhost");
            const user1 = new WebSocketDecorator(sandbox.spy() as any, ws1);

            expect(user1).to.exist;
        });
    });

    describe("properties", function() {
        let sandbox: sinon.SinonSandbox;

        beforeEach(function() {
            sandbox = sinon.sandbox.create();
        });

        afterEach(function() {
            sandbox.restore();
        });

        it("should return the websocket", function() {
            const ws1 = new FakeWebSocket("ws://localhost");
            const user1 = new WebSocketDecorator(sandbox.spy() as any, ws1);

            expect(user1.getWs()).to.equal(ws1);
        });
    });

    describe("broadcast", function() {
        let sandbox: SinonSandbox;
        const roomId1 = "test";
        const roomId2 = "test2";
        const data = "test";

        beforeEach(function() {
            sandbox = sinon.sandbox.create();
        });

        afterEach(function() {
            sandbox.restore();
        });

        it("should broadcast to the room", function() {
            const server = new WebSocketServer(http.createServer((req, res) => {
            }));
            const ws = new FakeWebSocket("ws://localhost");
            const user1 = new WebSocketDecorator(server, ws);
            server.joinRoom(roomId1, user1);

            const room = server.findRoom(roomId1);
            const roomSpy = sinon.spy(room, "broadcast");

            user1.broadcast.to(roomId1).send(data);

            expect(roomSpy.withArgs(data, user1).calledOnce).to.be.true;
        });

        it("should broadcast to all clients, across all rooms", function() {
            const server = new FakeWebSocketServer(http.createServer((req, res) => {
            }));
            const ws1 = new FakeWebSocket("ws://localhost");
            const ws2 = new FakeWebSocket("ws://localhost");

            server.addClient(ws1); //Hack around the WebSocket Server and directly add the clients
            server.addClient(ws2);

            const wsSpy1 = sandbox.spy(ws1, "send");
            const wsSpy2 = sandbox.spy(ws2, "send");

            const user1 = new WebSocketDecorator(server, ws1);
            const user2 = new WebSocketDecorator(server, ws2);

            server.joinRoom(roomId1, user1);
            server.joinRoom(roomId2, user2);

            user1.broadcast.send(data);

            expect(wsSpy1.withArgs(data).calledOnce).to.be.true;
            expect(wsSpy2.withArgs(data).calledOnce).to.be.true;
        });
    });

    describe("subscriptions", function() {
        let sandbox: SinonSandbox;
        const roomId = "test";
        let server: FakeWebSocketServer;
        beforeEach(function() {
            sandbox = sinon.sandbox.create();
            server = new FakeWebSocketServer(http.createServer((req, res) => {
            }));
        });

        afterEach(function() {
            sandbox.restore();
        });

        it("should ask the server to add a user to a room", function() {
            const ws1 = new FakeWebSocket("ws://localhost");

            const serverSpy = sinon.spy(server, "joinRoom");

            const user1 = new WebSocketDecorator(server, ws1);

            user1.join(roomId);

            expect(serverSpy.withArgs(roomId, user1).calledOnce).to.be.true;
        });

        it("should ask the server to remove a user of a room", function() {
            const ws1 = new FakeWebSocket("ws://localhost");

            const serverSpy = sinon.spy(server, "leaveRoom");

            const user1 = new WebSocketDecorator(server, ws1);

            user1.leave(roomId); //Should not crash even if the user was never in that room, or the room doesn't exist

            expect(serverSpy.withArgs(roomId, user1).calledOnce).to.be.true;
        });
    });
});
