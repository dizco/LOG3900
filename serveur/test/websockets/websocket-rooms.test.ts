import { Room } from "../../src/websockets/room";
import { WebSocketDecorator } from "../../src/decorators/websocket-decorator";
import { expect } from "chai";
import * as sinon from "sinon";
import { SinonSandbox } from "sinon";
import { FakeWebSocket } from "./fake-websocket";

describe("rooms", function() {
    describe("creation", function() {
        it("should create a new room", function() {
            const room = new Room("test");
            expect(room).to.not.be.undefined;
        });
    });

    describe("properties", function() {
        let sandbox: SinonSandbox;
        const roomId = "test";
        let room: Room;
        beforeEach(function() {
            sandbox = sinon.sandbox.create();
            room = new Room(roomId);
        });

        afterEach(function() {
            sandbox.restore();
        });

        it("should get the id", function() {
            expect(room.getId()).to.be.equal(roomId);
        });

        it("should contain no clients", function() {
            expect(room.getClients()).to.be.empty;
        });

        it("should be empty", function() {
            expect(room.isEmpty()).to.be.true;
        });

        it("should not be empty", function() {
            const ws1 = new FakeWebSocket("ws://localhost");
            const user1 = new WebSocketDecorator(sandbox.spy() as any, ws1);

            room.addClient(user1);
            expect(room.isEmpty()).to.be.false;
        });
    });

    describe("clients management", function() {
        let sandbox: SinonSandbox;
        const roomId = "test";
        let room: Room;
        beforeEach(function() {
            sandbox = sinon.sandbox.create();
            room = new Room(roomId);
        });

        afterEach(function() {
            sandbox.restore();
        });

        it("should add a client", function() {
            const ws1 = new FakeWebSocket("ws://localhost");
            const user1 = new WebSocketDecorator(sandbox.spy() as any, ws1);

            room.addClient(user1);
            expect(room.getClients()).to.deep.equal([user1]);
        });

        it("should not add an existing client", function() {
            const ws1 = new FakeWebSocket("ws://localhost");
            const user1 = new WebSocketDecorator(sandbox.spy() as any, ws1);

            room.addClient(user1);
            room.addClient(user1);
            expect(room.getClients()).to.deep.equal([user1]);
        });

        it("should remove a client", function() {
            const ws1 = new FakeWebSocket("ws://localhost");
            const ws2 = new FakeWebSocket("ws://localhost");

            const user1 = new WebSocketDecorator(sandbox.spy() as any, ws1);
            const user2 = new WebSocketDecorator(sandbox.spy() as any, ws2);

            room.addClient(user1);
            room.addClient(user2);
            expect(room.getClients()).to.deep.equal([user1, user2]);
            room.removeClient(user1);
            expect(room.getClients()).to.deep.equal([user2]);
        });
    });

    describe("broadcast", function() {
        let sandbox: SinonSandbox;

        let room: Room;
        const roomId = "test";
        const data = "test";

        beforeEach(function() {
            sandbox = sinon.sandbox.create();

            room = new Room(roomId);
        });

        afterEach(function() {
            sandbox.restore();
        });

        it("should broadcast to other users of the room", function() {
            const ws1 = new FakeWebSocket("ws://localhost");
            const ws2 = new FakeWebSocket("ws://localhost");
            const ws3 = new FakeWebSocket("ws://localhost");

            const wsSpy1 = sandbox.spy(ws1, "send");
            const wsSpy2 = sandbox.spy(ws2, "send");
            const wsSpy3 = sandbox.spy(ws3, "send");

            const user1 = new WebSocketDecorator(sandbox.spy() as any, ws1);
            const user2 = new WebSocketDecorator(sandbox.spy() as any, ws2);
            const user3 = new WebSocketDecorator(sandbox.spy() as any, ws3);

            room.addClient(user1);
            room.addClient(user2);
            room.addClient(user3);

            room.broadcast(data, user1);

            expect(wsSpy1.called).to.be.false;
            expect(wsSpy2.withArgs(data).calledOnce).to.be.true;
            expect(wsSpy3.withArgs(data).calledOnce).to.be.true;
        });
    });
});
