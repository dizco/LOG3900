import * as BluebirdPromise from "bluebird";

export class PromiseFactory {
    private static readonly DEFAULT_TIMEOUT = 5000;

    public static createTimeoutPromise<T>(callback: (resolve: (thenableOrResult?: T | PromiseLike<T>) => void, reject: (error?: any) => void, onCancel?: (callback: () => void) => void) => void)
        : BluebirdPromise<T> {
        const promise = new BluebirdPromise(callback);
        promise.timeout(PromiseFactory.DEFAULT_TIMEOUT);
        return promise;
    }
}
