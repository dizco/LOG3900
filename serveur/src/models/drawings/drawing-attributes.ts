import { Owner } from "./owner";

export interface DrawingAttributes {
    id: number | string;
    name: string;
    protected: boolean;
    owner: Owner;
}
