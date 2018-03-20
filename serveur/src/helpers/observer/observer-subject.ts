import { Observer } from "./observer";

//See https://github.com/torokmark/design_patterns_in_typescript
export class ObserverSubject {
    private observers: Observer[] = [];

    public register(observer: Observer): void {
        this.observers.push(observer);
    }

    public unregister(observer: Observer): void {
        this.observers.splice(this.observers.indexOf(observer), 1);
    }

    public notify(): void {
        this.observers.forEach((observer: Observer) => {
            if (observer) { //Null check in case the observer is garbage collected
                observer.notify();
            }
        });
    }
}