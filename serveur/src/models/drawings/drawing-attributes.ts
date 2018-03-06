import { Owner } from "./owner";

export interface DrawingAttributes {
    id: number | string;
    name: string;
    protection: { active: boolean, password?: string };
    owner: Owner;
}
