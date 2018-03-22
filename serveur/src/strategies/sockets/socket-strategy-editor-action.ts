import { SocketStrategy } from "../../models/sockets/socket-strategy";
import { ClientEditorAction } from "../../models/sockets/client-editor-action";
import { default as User, UserModel } from "../../models/User";
import { ProcessTimer } from "../../helpers/process-timer";
import { Command } from "../../helpers/queue/command";
import { QueueObserver } from "../../helpers/queue/queue-observer";
import { Observer } from "../../helpers/observer/observer";
import { PriorityQueue, QueueElementPriority } from "../../helpers/queue/priority-queue";
import { WebSocketDecorator } from "../../decorators/websocket-decorator";
import { ServerEditorAction } from "../../models/sockets/server-editor-action";
import { EditorActionDecorator } from "../../decorators/editor-action-decorator";
import Drawing, { DrawingModel } from "../../models/drawings/drawing";

export abstract class SocketStrategyEditorAction implements SocketStrategy {
    protected static queue: PriorityQueue<Command> = new PriorityQueue<Command>();
    protected static queueObserver: Observer = new QueueObserver(SocketStrategyEditorAction.queue); //The queue runs as soon as something is enqueued

    protected clientAction: ClientEditorAction;

    public constructor(clientAction: ClientEditorAction) {
        this.clientAction = clientAction;
    }

    /**
     * Decorate the message received by adding info, then broadcast to others in the room (drawing)
     * @param {WebSocketDecorator} wsDecorator
     */
    public execute(wsDecorator: WebSocketDecorator): void {
        const decorator = this.buildEditorActionDecorator(wsDecorator);
        const message = decorator.decorate(wsDecorator.getWs());

        //TODO: Validate if user is allowed to broadcast to that room
        const success = wsDecorator.broadcast.to(message.drawing.id.toString()).send(JSON.stringify(message));
        if (success) {
            this.saveVisualChanges(message);
            SocketStrategyEditorAction.saveAction(message);
        }
        else {
            console.log("EditorAction failed to broadcast");
            //TODO: Notify emitting user
        }
    }

    protected abstract buildEditorActionDecorator(wsDecorator: WebSocketDecorator): EditorActionDecorator;

    protected abstract saveVisualChanges(message: ServerEditorAction): void;

    private static saveAction(message: ServerEditorAction): void {
        this.queue.enqueue(this.buildActionCommand(message), QueueElementPriority.Low);
    }

    private static buildActionCommand(message: ServerEditorAction): Command {
        return new Command("Update Drawing Actions: SaveAction", () => {
            return new Promise<boolean>((resolve: (value?: boolean | PromiseLike<boolean>) => void,
                                         reject: (reason?: any) => void) => {
                const timer = new ProcessTimer();
                timer.start();
                User.findOne({ username: message.author.username }, (err: any, user: UserModel) => {
                    if (err || !user) {
                        console.log("An error occurred while saving a user action", err, user);
                        return reject(err);
                    }
                    else {
                        const action = { actionId: message.action.id, name: message.action.name, author: user, timestamp: message.timestamp };
                        Drawing.findByIdAndUpdate(message.drawing.id, { $push: { actions: action }}, (err: any, drawing: DrawingModel) => {
                            timer.stop();
                            timer.print("Update Drawing Actions: SaveAction");
                            if (err) {
                                console.log(`SaveAction: An error occurred while adding action`, err);
                                return reject(err);
                            }
                            return resolve(true);
                        });
                    }
                });
            });
        });
    }
}