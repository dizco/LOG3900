import { Dot } from "./dot";

export interface Stroke {
    author: {
        id: number | string;
    };

    dots: Dot[];
}
