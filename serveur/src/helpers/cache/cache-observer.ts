import { Observer } from "../observer/observer";
import { CacheAside } from "./cache-aside";
import { default as chalk } from "chalk";

export class CacheObserver<T> extends Observer {
    private static readonly CLEANUP_INTERVAL = 60000;
    private static readonly AUTOSAVE_INTERVAL = 30000;
    private static readonly CACHE_ITEM_LIFETIME_SECONDS = 90;
    private running: boolean;
    private cleanupInterval: NodeJS.Timer;
    private autosaveInterval: NodeJS.Timer;

    constructor (private cache: CacheAside<T>, private autosave: () => void = () => {}) {
        super();
        this.running = false;

        //Never unregisters because the CacheObserver is expected to be static
        //Could potentially use something like this https://stackoverflow.com/questions/28885625/is-there-destructor-in-typescript
        //And then we could initialize the CacheObserver somewhere and also release it when necessary. The release would unregister
        this.cache.register(this);
    }

    public notify(): void {
        if (this.running) {
            if (this.cache.isEmpty()) {
                this.stopWatching();
            }
        }
        else { //Avoid having two intervals at once
            this.startWatching();
        }
    }

    private startWatching(): void {
        if (this.cache.isEmpty()) {
            this.stopWatching();
            return;
        }

        this.running = true;
        this.cleanupInterval = setInterval(this.cleanup, CacheObserver.CLEANUP_INTERVAL);
        this.autosaveInterval = setInterval(this.autosave, CacheObserver.AUTOSAVE_INTERVAL);
    }

    private stopWatching(): void {
        this.running = false;
        clearInterval(this.cleanupInterval);
        clearInterval(this.autosaveInterval);
    }

    private cleanup = () => {
        const initialSize = this.cache.size();
        const numberOfItems = this.cache.clearOlderThan(CacheObserver.CACHE_ITEM_LIFETIME_SECONDS);
        if (process.env.NODE_ENV === "development") {
            console.log(`${chalk.cyan("[CacheObserver]")} Cleared ${numberOfItems} items from cache. ${initialSize - numberOfItems} items left.`);
        }
    };
}
