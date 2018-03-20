export interface Queue<T> {
    enqueue(val: T): void;
    dequeue(): T | undefined;
    isEmpty(): boolean;
    peek(): T | undefined;
    size(): number;
    clear(): void;
}
