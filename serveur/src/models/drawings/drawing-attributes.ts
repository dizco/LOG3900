import { Author } from "../author";

export interface DrawingAttributes {
    id: number | string;
    name: string;
    owner: Author;
}