import { WebSocketDecorator } from "../../decorators/websocket-decorator";
import { ClientStrokeEditorAction } from "../../models/sockets/client-editor-action";
import { SocketStrategyEditorAction } from "./socket-strategy-editor-action";
import { StrokeEditorActionDecorator } from "../../decorators/stroke-editor-action-decorator";
import { EditorActionDecorator } from "../../decorators/editor-action-decorator";
import { Command } from "../../helpers/queue/command";
import { default as Drawing, DrawingModel } from "../../models/drawings/drawing";
import { InsertAllAtIndex } from "../../helpers/arrays";
import { ProcessTimer } from "../../helpers/process-timer";
import { Stroke } from "../../models/drawings/stroke";
import { ServerStrokeEditorAction } from "../../models/sockets/server-editor-action";
import { PromiseFactory } from "../../factories/promise-factory";
import { DrawingsCache } from "../../helpers/cache/drawings-cache-singleton";

const enum StrokeEditorAction {
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
        switch (message.action.id) {
            case StrokeEditorAction.NewStroke:
                SocketStrategyEditorAction.queue.enqueue(SocketStrategyStrokeEditorAction.buildNewStrokeCommand(message));
                break;
            case StrokeEditorAction.ReplaceStroke:
                SocketStrategyEditorAction.queue.enqueue(SocketStrategyStrokeEditorAction.buildReplaceStrokeCommand(message));
                break;
            case StrokeEditorAction.Transform:
                SocketStrategyEditorAction.queue.enqueue(SocketStrategyStrokeEditorAction.buildTransformCommand(message));
                break;
            case StrokeEditorAction.Reset:
                SocketStrategyEditorAction.queue.clear(message.drawing.id);
                SocketStrategyEditorAction.queue.enqueue(SocketStrategyStrokeEditorAction.buildResetCommand(message));
                break;
            default:
                console.log(`Editor Action (id ${message.action.id}, name ${message.action.name}) does not require strokes manipulation.`);
        }
    }

    private static buildNewStrokeCommand(message: ServerStrokeEditorAction): Command {
        return new Command("StrokeEditorAction: NewStroke", message.drawing.id, () => {
            return PromiseFactory.createTimeoutPromise<boolean>((resolve: (value?: boolean | PromiseLike<boolean>) => void,
                                                                 reject: (reason?: any) => void) => {
                const timer = new ProcessTimer();
                timer.start();

                DrawingsCache.getInstance().getById(message.drawing.id)
                    .then(drawing => {
                        drawing.strokes.push(...message.delta.add);

                        timer.stop();
                        timer.print("Save Editor Strokes: NewStroke");
                        return resolve(true);
                    })
                    .catch(err => {
                        console.log("NewStroke: An error occurred while fetching drawing", err);
                        return reject(err);
                    });
            });
        });
    }

    private static buildReplaceStrokeCommand(message: ServerStrokeEditorAction): Command {
        return new Command("StrokeEditorAction: ReplaceStroke", message.drawing.id, () => {
            return PromiseFactory.createTimeoutPromise<boolean>((resolve: (value?: boolean | PromiseLike<boolean>) => void,
                                         reject: (reason?: any) => void) => {
                const timer = new ProcessTimer();
                timer.start();

                DrawingsCache.getInstance().getById(message.drawing.id)
                    .then(drawing => {
                        //There is only one remove when ReplaceStroke
                        const removeIndex = drawing.strokes.findIndex((stroke: Stroke) => stroke.strokeUuid === message.delta.remove[0]);
                        if (removeIndex > -1) {
                            if (process.env.NODE_ENV === "development") {
                                console.log(`ReplaceStroke: Removing stroke (id ${message.delta.remove[0]}).`);
                            }
                            drawing.strokes.splice(removeIndex, 1);
                            if (message.delta.add && message.delta.add.length > 0) {
                                drawing.strokes = InsertAllAtIndex<Stroke>(drawing.strokes, removeIndex, ...message.delta.add);
                            }
                        }
                        else {
                            return reject(`ReplaceStroke: Could not find the stroke (id ${message.delta.remove[0]}) to be removed.`);
                        }

                        timer.stop();
                        timer.print("Save Editor Strokes: ReplaceStroke");
                        return resolve(true);
                    })
                    .catch(err => {
                        console.log("ReplaceStroke: An error occurred while fetching drawing", err);
                        return reject(err);
                    });
            });
        });
    }

    private static buildTransformCommand(message: ServerStrokeEditorAction): Command {
        return new Command("StrokeEditorAction: Transform", message.drawing.id, () => {
            return PromiseFactory.createTimeoutPromise<boolean>((resolve: (value?: boolean | PromiseLike<boolean>) => void,
                                         reject: (reason?: any) => void) => {
                const timer = new ProcessTimer();
                timer.start();

                DrawingsCache.getInstance().getById(message.drawing.id)
                    .then(drawing => {
                        //Replace attributes of strokes in the "Add". The modified strokes are all in the "Add".
                        message.delta.add.forEach((transformedStroke: Stroke) => {
                            const savedStroke = drawing.strokes.find((stroke: Stroke) => stroke.strokeUuid === transformedStroke.strokeUuid);
                            savedStroke.strokeAttributes = transformedStroke.strokeAttributes;
                            savedStroke.dots = transformedStroke.dots;
                        });

                        timer.stop();
                        timer.print("Save Editor Strokes: Transform");
                        return resolve(true);
                    })
                    .catch(err => {
                        console.log("Transform: An error occurred while fetching drawing", err);
                        return reject(err);
                    });
            });
        });
    }

    private static buildResetCommand(message: ServerStrokeEditorAction): Command {
        return new Command("StrokeEditorAction: Reset", message.drawing.id, () => {
            return PromiseFactory.createTimeoutPromise<boolean>((resolve: (value?: boolean | PromiseLike<boolean>) => void,
                                                                 reject: (reason?: any) => void) => {
                const timer = new ProcessTimer();
                timer.start();

                DrawingsCache.getInstance().getById(message.drawing.id)
                    .then(drawing => {
                        drawing.strokes = [];

                        timer.stop();
                        timer.print("Save Editor Strokes: Reset");
                        return resolve(true);
                    })
                    .catch(err => {
                        console.log("Reset: An error occurred while fetching drawing", err);
                        return reject(err);
                    });
            });
        });
    }
}