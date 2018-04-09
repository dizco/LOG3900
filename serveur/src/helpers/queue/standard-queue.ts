import { Queue } from "./queue";
import { TaggedObject } from "./tagged-object";

export class StandardQueue<T extends TaggedObject> implements Queue<T> {
    private queue: T[] = [];
    private offset = 0; //Delimits number of empty spaces in the beginning of the array

    public enqueue(val: T): void {
        this.queue.push(val);
    }

    public dequeue(): T | undefined {
        if (this.isEmpty()) {
            return undefined;
        }

        // store the item at the front of the queue
        const item = this.queue[this.offset];

        // increment the offset and remove the free space if necessary
        if ((++this.offset) * 2 >= this.queue.length) {
            this.queue = this.queue.slice(this.offset);
            this.offset = 0;
        }

        return item;
    }

    public isEmpty(): boolean {
        return this.queue.length === 0;
    }

    public peek(): T | undefined {
        return !this.isEmpty() ? this.queue[this.offset] : undefined;
    }

    public size(): number {
        return this.queue.length - this.offset;
    }

    public clear(tag?: string): void {
        if (tag) {
            this.queue = this.queue.filter((item: T) => {
                return item.getTag() !== tag; //Remove items that have given tag
            });
        }
        else {
            this.queue = [];
        }
        this.offset = 0;
    }
}
