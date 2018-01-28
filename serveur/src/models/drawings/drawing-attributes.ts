import { Owner } from "./owner";

export interface DrawingAttributes {
    id: number | string;
    name: string;
    owner: Owner;
}