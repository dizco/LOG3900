import { Pixel } from "./pixel";

export interface Stroke {
    author: {
        id: number | string;
    };

    dots: Pixel[];
}
