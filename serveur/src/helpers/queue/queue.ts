import { TaggedObject } from "./tagged-object";

export interface Queue<T extends TaggedObject> {
    enqueue(val: T): void;
    dequeue(): T | undefined;
    isEmpty(): boolean;
    peek(): T | undefined;
    size(): number;
    clear(tag?: string): void;
}
