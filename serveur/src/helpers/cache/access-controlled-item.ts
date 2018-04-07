export class AccessControlledItem<T> {
    private item: T;
    private lastAccess: Date;

    public constructor(item: T) {
        this.Item = item;
    }

    public get Item(): T {
        this.lastAccess = new Date();
        return this.item;
    }

    public set Item(item: T) {
        this.lastAccess = new Date();
        this.item = item;
    }

    /**
     * Get the item without updating the lastAccess
     * @returns {T}
     */
    public getIncognito(): T {
        return this.item;
    }

    /**
     * @returns {number} time in seconds since last access
     */
    public timeElapsedSinceLastAccess(): number {
        return Math.floor((new Date().getTime() - this.lastAccess.getTime()) / 1000);
    }
}
