import { Owner } from "./owner";

export interface DrawingAttributes {
    id: number | string;
    name: string;
    mode: "stroke" | "pixel";
    visibility: "public" | "private";
    protection: { active: boolean, password?: string };
    owner: Owner;
}
