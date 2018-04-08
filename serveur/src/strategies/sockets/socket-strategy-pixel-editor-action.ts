import { WebSocketDecorator } from "../../decorators/websocket-decorator";
import { ClientPixelEditorAction } from "../../models/sockets/client-editor-action";
import { SocketStrategyEditorAction } from "./socket-strategy-editor-action";
import { PixelEditorActionDecorator } from "../../decorators/pixel-editor-action-decorator";
import { EditorActionDecorator } from "../../decorators/editor-action-decorator";
import { ServerPixelEditorAction } from "../../models/sockets/server-editor-action";
import { Command } from "../../helpers/queue/command";
import { ProcessTimer } from "../../helpers/process-timer";
import { ColorPixel } from "../../models/drawings/pixel";
import { PromiseFactory } from "../../factories/promise-factory";
import * as BluebirdPromise from "bluebird";
import { DrawingsCache } from "../../helpers/cache/drawings-cache-singleton";
import { Dictionary } from "../../helpers/dictionary";
import { QueueElementPriority } from "../../helpers/queue/priority-queue";

const enum PixelEditorAction {
    NewPixels = 1,
    LockPixels = 2,
    UnlockPixels = 3,
}

class CleanupCommand extends Command {
    private drawingId: string;

    public constructor(drawingId: string, description: string, command: () => BluebirdPromise<boolean>) {
        super(description, drawingId, command);
        this.drawingId = drawingId;
    }

    public getDrawingId(): string {
        return this.drawingId;
    }
}

export class SocketStrategyPixelEditorAction extends SocketStrategyEditorAction {
    private static readonly CLEANUP_DELAY = 10000;
    private static cleanupCommands: CleanupCommand[] = [];
    protected clientAction: ClientPixelEditorAction;

    public constructor(clientAction: ClientPixelEditorAction) {
        super(clientAction);
        this.clientAction = clientAction;
    }

    protected buildEditorActionDecorator(wsDecorator: WebSocketDecorator): EditorActionDecorator {
        return new PixelEditorActionDecorator(this.clientAction, wsDecorator.user);
    }

    protected saveVisualChanges(message: ServerPixelEditorAction): void {
        switch (message.action.id) {
            case PixelEditorAction.NewPixels:
                if (!(message.pixels.length > 0)) { //No pixels have changed
                    console.log(`Editor Action (id ${message.action.id}, name ${message.action.name}) has no pixels changes.`);
                    return;
                }
                SocketStrategyEditorAction.queue.enqueue(SocketStrategyPixelEditorAction.buildNewPixelsCommand(message));
                if (!SocketStrategyPixelEditorAction.cleanupInProgress(message.drawing.id)) {
                    const command = SocketStrategyPixelEditorAction.buildCleanupRedundantPixelsCommand(message.drawing.id);
                    SocketStrategyPixelEditorAction.cleanupCommands.push(command);
                    setTimeout(() => {
                        SocketStrategyEditorAction.queue.enqueue(command, QueueElementPriority.Low);
                    }, SocketStrategyPixelEditorAction.CLEANUP_DELAY);
                }
                break;
            default:
                console.log(`Editor Action (id ${message.action.id}, name ${message.action.name}) does not require pixels manipulation.`);
        }
    }

    private static cleanupInProgress(drawingId: string): boolean {
        return SocketStrategyPixelEditorAction.cleanupCommands
            .filter((command: CleanupCommand) => command.getDrawingId() === drawingId)
            .length > 0;
    }

    private static removeCleanupCommand(drawingId: string): void {
        SocketStrategyPixelEditorAction.cleanupCommands = SocketStrategyPixelEditorAction.cleanupCommands
            .filter((command: CleanupCommand) => command.getDrawingId() !== drawingId);
    }

    private static buildNewPixelsCommand(message: ServerPixelEditorAction): Command {
        return new Command("PixelEditorAction: NewPixels", message.drawing.id, () => {
            return PromiseFactory.createTimeoutPromise<boolean>((resolve: (value?: boolean | PromiseLike<boolean>) => void,
                                         reject: (reason?: any) => void) => {
                const timer = new ProcessTimer();
                timer.start();

                DrawingsCache.getInstance().getById(message.drawing.id)
                    .then(drawing => {
                        drawing.pixels.push(...message.pixels);

                        timer.stop();
                        timer.print("Save Editor Pixels: NewPixels");
                        return resolve(true);
                    })
                    .catch(err => {
                        console.log("NewPixels: An error occurred while fetching drawing", err);
                        return reject(err);
                    });
            });
        });
    }

    private static buildCleanupRedundantPixelsCommand(drawingId: string): CleanupCommand {
        return new CleanupCommand(drawingId, "PixelEditorAction: CleanupPixels", () => {
            return PromiseFactory.createTimeoutPromise<boolean>((resolve: (value?: boolean | PromiseLike<boolean>) => void,
                                         reject: (reason?: any) => void) => {
                const timer = new ProcessTimer();
                timer.start();

                DrawingsCache.getInstance().getById(drawingId)
                    .then(drawing => {
                        //Push every pixel to a "x,y" dictionary so that most recent pixels will overwrite older pixels
                        const dictionary: Dictionary<ColorPixel> = {};
                        drawing.pixels.forEach((pixel: ColorPixel) => {
                            dictionary["" + pixel.x + "," + pixel.y] = pixel;
                        });

                        drawing.pixels = [];
                        Object.keys(dictionary).forEach((key) => {
                            drawing.pixels.push(dictionary[key]);
                        });

                        timer.stop();
                        timer.print("Save Editor Pixels: NewPixels");
                        SocketStrategyPixelEditorAction.removeCleanupCommand(drawingId);
                        return resolve(true);
                    })
                    .catch(err => {
                        console.log("NewPixels: An error occurred while fetching drawing", err);
                        SocketStrategyPixelEditorAction.removeCleanupCommand(drawingId);
                        return reject(err);
                    });
            });
        });
    }
}