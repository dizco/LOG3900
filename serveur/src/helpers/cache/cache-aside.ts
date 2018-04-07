import { Dictionary } from "../dictionary";
import { AccessControlledItem } from "./access-controlled-item";
import { ObserverSubject } from "../observer/observer-subject";

export class CacheAside<T> extends ObserverSubject {
    protected cache: Dictionary<AccessControlledItem<T>> = {};

    public add(key: string, item: T): void {
        this.cache[key] = new AccessControlledItem<T>(item);
        this.notify();
    }

    public async get(key: string, dataRetriever: () => Promise<T>): Promise<T> {
        const cacheItem = this.cache[key];
        if (cacheItem !== undefined) {
            return cacheItem.Item;
        }
        const promise = dataRetriever();
        promise.then((retrievedItem: T) => {
            this.add(key, retrievedItem);
        });
        return promise;
    }

    /**
     * Get an array of the currently cached items, without updating the last access to an item
     * @returns {T[]}
     */
    public getCachedItems(): T[] {
        return Object.keys(this.cache).map((key) => {
            return this.cache[key].getIncognito();
        });
    }

    public remove(key: string): void {
        this.beforeRemove(key)
            .then(() => {
                delete this.cache[key];
                this.notify();
            });
    }

    public size(): number {
        return Object.keys(this.cache).length;
    }

    public isEmpty(): boolean {
        return this.size() === 0;
    }

    /**
     * Removes cache items older than specified number of seconds
     * @param {number} seconds
     * @returns {number} number of items removed
     */
    public clearOlderThan(seconds: number): number {
        let numberRemoved = 0;
        Object.keys(this.cache).forEach((key) => {
            if (this.cache[key].timeElapsedSinceLastAccess() > seconds) {
                numberRemoved++;
                this.remove(key);
            }
        });

        return numberRemoved;
    }

    /**
     * Allows child classes to manipulate the item before it gets removed
     * @param {string} key
     * @returns {Promise<void>}
     */
    protected beforeRemove(key: string): Promise<void> {
        return Promise.resolve();
    }
}
