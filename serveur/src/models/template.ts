import * as mongoose from "mongoose";
import * as mongoosePaginate from "mongoose-paginate";
import { ColorPixel } from "./drawings/pixel";
import { Stroke } from "./drawings/stroke";
import { Owner } from "./drawings/owner";

export interface TemplateInterface {
    id: number | string;
    name: string;
    mode: "stroke" | "pixel";
    owner: Owner;
    thumbnail: string;
    strokes?: Stroke[];
    pixels?: ColorPixel[];
}

export type TemplateModel = mongoose.Document & TemplateInterface;

const templateSchema = new mongoose.Schema({
    name: { type: String, required: true },
    mode: { type: String, enum: ["stroke", "pixel"], required: true },
    owner: { type: mongoose.Schema.Types.ObjectId, ref: "User", required: true },
    strokes: [{ _id: false,
        strokeUuid: String,
        strokeAttributes: { _id: false, color: String, height: Number, width: Number, stylusTip: String },
        dots: [{ _id: false, x: Number, y: Number }],
    }],
    pixels: [{ _id: false, x: Number, y: Number, color: String }],
    thumbnail: { type: String, default: "" },
}, { timestamps: true });
templateSchema.plugin(mongoosePaginate);

if (!(<any>templateSchema).options.toObject) (<any>templateSchema).options.toObject = {};
(<any>templateSchema).options.toObject.transform = function (doc: any, ret: any, options: any) {
    //ret.id = ret._id;
    //delete ret._id;
    return ret;
};

const Template = mongoose.model<TemplateModel>("Template", templateSchema);
export default Template;