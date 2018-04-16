import { default as Drawing, DrawingModel } from "../../models/drawings/drawing";
import { CacheAside } from "./cache-aside";
import { CacheKeyFactory } from "../../factories/cache-key-factory";
import { default as chalk } from "chalk";
import { AccessControlledItem } from "./access-controlled-item";
import { CacheObserver } from "./cache-observer";
import { ProcessTimer } from "../process-timer";

export class DrawingsCache extends CacheAside<DrawingModel> {
    private static instance: DrawingsCache;
    private static cacheObserver: CacheObserver<DrawingModel>;
    public static getInstance(): DrawingsCache {
        if (!DrawingsCache.instance) {
            DrawingsCache.instance = new DrawingsCache();
            DrawingsCache.cacheObserver = new CacheObserver<DrawingModel>(DrawingsCache.instance, DrawingsCache.instance.autosave);
        }
        return DrawingsCache.instance;
    }

    /**
     * Adds a DrawingModel to the cache
     * @param {DrawingModel} drawing
     */
    public addModel(drawing: DrawingModel): void {
        return super.add(CacheKeyFactory.createDrawingCacheKey(drawing._id), drawing);
    }

    /**
     * Fetches drawing from cache if present, otherwise fetches from database
     * @param {string} drawingId
     * @returns {Promise<DrawingModel>}
     */
    public getById(drawingId: string): Promise<DrawingModel> {
        const dataRetriever = () => {
            return new Promise<DrawingModel>((resolve: (value?: DrawingModel | PromiseLike<DrawingModel>) => void,
                                       reject: (reason?: any) => void) => {
                console.log(`${chalk.cyan("[DrawingsCache]")} Cache miss. Fetching drawing (id ${drawingId}) from database.`);

                const populateOptions = [
                    { path: "owner" },
                ];
                Drawing.findOne({ _id: drawingId }, { thumbnail: 0 }).populate(populateOptions).exec((err: any, drawing: DrawingModel) => {
                    if (err || !drawing) {
                        console.log(`${chalk.cyan("[DrawingsCache]")} Fetch drawing errored. ${err}`);
                        return reject(err);
                    }

                    return resolve(drawing);
                });
            });
        };
        return super.get(CacheKeyFactory.createDrawingCacheKey(drawingId), dataRetriever);
    }

    /**
     * Removes the drawing from cache
     * @param {string} drawingId
     */
    public removeById(drawingId: string): void {
        return super.remove(CacheKeyFactory.createDrawingCacheKey(drawingId));
    }

    /**
     * Override base method, save drawing to database before removing
     * @param {string} key
     * @returns {Promise<void>}
     */
    protected beforeRemove(key: string): Promise<void> {
        const timer = new ProcessTimer();
        timer.start();

        const drawing = (<AccessControlledItem<DrawingModel>>this.cache[key]).Item;
        return drawing.save()
            .then(() => {
                timer.stop();
                timer.print("Save Drawing to Database: DrawingsCache");
                return Promise.resolve();
            }).catch((err: any) => {
                console.log(`${chalk.cyan("[DrawingsCache]")} Save drawing errored. ${err}`);
                return Promise.reject(err);
            });
    }

    private autosave = () => {
        this.getCachedItems().forEach((drawing: DrawingModel) => {
            const timer = new ProcessTimer();
            timer.start();

            drawing.save()
                .then(() => {
                    timer.stop();
                    timer.print("Autosave Drawing to Database: DrawingsCache");
                }).catch((err: any) => {
                    console.log(`${chalk.cyan("[DrawingsCache]")} Autosave drawing (id ${drawing._id}) errored. ${err}`);
                });
        });
    };
}
