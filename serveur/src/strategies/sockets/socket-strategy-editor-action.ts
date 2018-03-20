import { SocketStrategy } from "../../models/sockets/socket-strategy";
import { WebSocketDecorator } from "../../decorators/websocket-decorator";
import { EditorActionDecorator } from "../../decorators/editor-action-decorator";
import { ClientEditorAction } from "../../models/sockets/client-editor-action";
import { ServerEditorAction } from "../../models/sockets/server-editor-action";
import { default as Drawing, DrawingModel } from "../../models/drawings/drawing";
import { default as User, UserModel } from "../../models/User";
import { ProcessTimer } from "../../helpers/process-timer";
import { Stroke } from "../../models/drawings/stroke";
import { InsertAllAtIndex } from "../../helpers/arrays";
import { Command } from "../../helpers/queue/command";
import { QueueObserver } from "../../helpers/queue/queue-observer";
import { Observer } from "../../helpers/observer/observer";
import { PriorityQueue, QueueElementPriority } from "../../helpers/queue/priority-queue";

const enum EditorAction {
    NewStroke = 1,
    ReplaceStroke = 2,
    LockStrokes = 3,
    UnlockStrokes = 4,
    Transform = 5,
    Reset = 6,
}

export class SocketStrategyEditorAction implements SocketStrategy {
    private static queue: PriorityQueue<Command> = new PriorityQueue<Command>();
    private static queueObserver: Observer = new QueueObserver(SocketStrategyEditorAction.queue); //The queue runs as soon as something is enqueued

    private clientAction: ClientEditorAction;

    public constructor(clientAction: ClientEditorAction) {
        this.clientAction = clientAction;
    }

    /**
     * Decorate the message received by adding info, then broadcast to others in the room (drawing)
     * @param {WebSocketDecorator} wsDecorator
     */
    public execute(wsDecorator: WebSocketDecorator): void {
        const decorator = new EditorActionDecorator(this.clientAction, wsDecorator.user);
        decorator.decorate(wsDecorator.getWs())
            .then((message: ServerEditorAction) => {
                //TODO: Validate if user is allowed to broadcast to that room
                const success = wsDecorator.broadcast.to(message.drawing.id.toString()).send(JSON.stringify(message));
                if (success) {
                    SocketStrategyEditorAction.saveStrokes(message);
                    SocketStrategyEditorAction.saveAction(message);
                }
                else {
                    console.log("EditorAction failed to broadcast");
                    //TODO: Notify emitting user
                }
            })
            .catch((reason => console.log("EditorAction failed to fetch db info", reason)));
    }

    private static saveAction(message: ServerEditorAction): void {
        this.queue.enqueue(this.buildActionCommand(message), QueueElementPriority.Low);
    }

    private static buildActionCommand(message: ServerEditorAction): Command {
        return new Command(() => {
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
                                console.log(`SaveAction: An error occurred while adding actiom`, err);
                                return reject(err);
                            }
                            return resolve(true);
                        });
                    }
                });
            });
        });
    }

    private static saveStrokes(message: ServerEditorAction): void {
        const conditions = { _id: message.drawing.id };
        switch (message.action.id) {
            case EditorAction.NewStroke:
                this.queue.enqueue(this.buildUpdateCommand("NewStroke", conditions,
                    { $push: { strokes: { $each: message.delta.add } }}));
                break;
            case EditorAction.ReplaceStroke:
                this.queue.enqueue(this.buildReplaceStrokeCommand(conditions, message));
                break;
            case EditorAction.Transform:
                this.queue.enqueue(this.buildTransformCommand(conditions, message));
                break;
            case EditorAction.Reset:
                this.queue.clear();
                this.queue.enqueue(this.buildUpdateCommand("Reset", conditions,
                    { $set: { strokes: [] }}));
                break;
            default:
                console.log(`Editor Action (id ${message.action.id}, name ${message.action.name}) does not require strokes manipulation.`);
        }
    }

    private static buildUpdateCommand(commandName: string, conditions: object, update: object): Command {
        return new Command(() => {
            return new Promise<boolean>((resolve: (value?: boolean | PromiseLike<boolean>) => void,
                                         reject: (reason?: any) => void) => {
                const timer = new ProcessTimer();
                timer.start();
                Drawing.update(conditions, update, (err: any, drawing: DrawingModel) => {
                    timer.stop();
                    timer.print(`Update Editor Strokes: ${commandName}`);
                    if (err) {
                        console.log(`${commandName}: An error occurred while updating strokes`, err);
                        return reject(err);
                    }
                    return resolve(true);
                });
            });
        });
    }

    private static buildReplaceStrokeCommand(conditions: object, message: ServerEditorAction): Command {
        return new Command(() => {
            return new Promise<boolean>((resolve: (value?: boolean | PromiseLike<boolean>) => void,
                                         reject: (reason?: any) => void) => {
                const timer = new ProcessTimer();
                timer.start();
                Drawing.findOne(conditions, {actions: 0}).exec((err: any, drawing: DrawingModel) => {
                    if (err || !drawing) {
                        console.log("ReplaceStroke: An error occurred while fetching drawing", err);
                        return reject(err);
                    }

                    //There is only one remove when ReplaceStroke
                    const removeIndex = drawing.strokes.findIndex((stroke: Stroke) => stroke.strokeUuid === message.delta.remove[0]);
                    if (removeIndex > -1) {
                        console.log(`ReplaceStroke: Removing stroke (id ${message.delta.remove[0]}).`);
                        drawing.strokes.splice(removeIndex, 1);
                        if (message.delta.add && message.delta.add.length > 0) {
                            drawing.strokes = InsertAllAtIndex<Stroke>(drawing.strokes, removeIndex, ...message.delta.add);
                        }
                    }
                    else {
                        return reject(`ReplaceStroke: Could not find the stroke (id ${message.delta.remove[0]}) to be removed.`);
                    }

                    drawing.save().then((value) => {
                            timer.stop();
                            timer.print("Save Editor Strokes: ReplaceStroke");
                            return resolve(true);
                        }).catch((err: any) => {
                            console.log("ReplaceStroke: An error occurred while saving drawing", err);
                            return reject(err);
                        });
                });

            });
        });
    }

    private static buildTransformCommand(conditions: object, message: ServerEditorAction): Command {
        return new Command(() => {
            return new Promise<boolean>((resolve: (value?: boolean | PromiseLike<boolean>) => void,
                                         reject: (reason?: any) => void) => {
                const timer = new ProcessTimer();
                timer.start();
                Drawing.findOne(conditions, {actions: 0}).exec((err: any, drawing: DrawingModel) => {
                    if (err || !drawing) {
                        console.log("Transform: An error occurred while fetching drawing", err);
                        return reject(err);
                    }

                    //Replace attributes of strokes in the "Add". The modified strokes are all in the "Add".
                    message.delta.add.forEach((transformedStroke: Stroke) => {
                        const savedStroke = drawing.strokes.find((stroke: Stroke) => stroke.strokeUuid === transformedStroke.strokeUuid);
                        savedStroke.strokeAttributes = transformedStroke.strokeAttributes;
                        savedStroke.dots = transformedStroke.dots;
                    });

                    drawing.save().then((value) => {
                        timer.stop();
                        timer.print("Save Editor Strokes: Transform");
                        return resolve(true);
                    }).catch((err: any) => {
                        console.log("Transform: An error occurred while saving drawing", err);
                        return reject(err);
                    });
                });

            });
        });
    }
}