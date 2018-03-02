import { DrawingAttributes } from "./drawing-attributes";
import { ServerEditorAction } from "../sockets/server-editor-action";
import * as mongoose from "mongoose";
import * as mongoosePaginate from "mongoose-paginate";

interface DrawingInterface extends DrawingAttributes {
    actions?: ServerEditorAction[];
}

export type DrawingModel = mongoose.Document & DrawingInterface;

const drawingSchema = new mongoose.Schema({
    name: String,
    owner: { type: mongoose.Schema.Types.ObjectId, ref: "User" },
    actions: [{ actionId: Number, name: String, author: { type: mongoose.Schema.Types.ObjectId, ref: "User" } }],
}, { timestamps: true });
drawingSchema.plugin(mongoosePaginate);

const Drawing = mongoose.model("Drawing", drawingSchema);
export default Drawing;