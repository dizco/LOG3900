import { WebSocketDecorator } from "../../decorators/websocket-decorator";
import { ClientStrokeEditorAction } from "../../models/sockets/client-editor-action";
import { SocketStrategyEditorAction } from "./socket-strategy-editor-action";
import { StrokeEditorActionDecorator } from "../../decorators/stroke-editor-action-decorator";
import { EditorActionDecorator } from "../../decorators/editor-action-decorator";
import { Command } from "../../helpers/queue/command";
import Drawing, { DrawingModel } from "../../models/drawings/drawing";
import { InsertAllAtIndex } from "../../helpers/arrays";
import { ProcessTimer } from "../../helpers/process-timer";
import { Stroke } from "../../models/drawings/stroke";
import { ServerStrokeEditorAction } from "../../models/sockets/server-editor-action";

const enum EditorAction {
    NewStroke = 1,
    ReplaceStroke = 2,
    LockStrokes = 3,
    UnlockStrokes = 4,
    Transform = 5,
    Reset = 6,
}

export class SocketStrategyStrokeEditorAction extends SocketStrategyEditorAction  {
    protected clientAction: ClientStrokeEditorAction;

    public constructor(clientAction: ClientStrokeEditorAction) {
        super(clientAction);
        this.clientAction = clientAction;
    }

    protected buildEditorActionDecorator(wsDecorator: WebSocketDecorator): EditorActionDecorator {
        return new StrokeEditorActionDecorator(this.clientAction, wsDecorator.user);
    }

    protected saveVisualChanges(message: ServerStrokeEditorAction): void {
        const conditions = { _id: message.drawing.id };
        switch (message.action.id) {
            case EditorAction.NewStroke:
                SocketStrategyEditorAction.queue.enqueue(SocketStrategyStrokeEditorAction.buildUpdateCommand("NewStroke", conditions,
                    { $push: { strokes: { $each: message.delta.add } }}));
                break;
            case EditorAction.ReplaceStroke:
                SocketStrategyEditorAction.queue.enqueue(SocketStrategyStrokeEditorAction.buildReplaceStrokeCommand(conditions, message));
                break;
            case EditorAction.Transform:
                SocketStrategyEditorAction.queue.enqueue(SocketStrategyStrokeEditorAction.buildTransformCommand(conditions, message));
                break;
            case EditorAction.Reset:
                SocketStrategyEditorAction.queue.clear();
                SocketStrategyEditorAction.queue.enqueue(SocketStrategyStrokeEditorAction.buildUpdateCommand("Reset", conditions,
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

    private static buildReplaceStrokeCommand(conditions: object, message: ServerStrokeEditorAction): Command {
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

    private static buildTransformCommand(conditions: object, message: ServerStrokeEditorAction): Command {
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