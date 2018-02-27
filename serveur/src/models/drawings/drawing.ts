import { DrawingAttributes } from "./drawing-attributes";
import { ServerEditorAction } from "../sockets/server-editor-action";
import * as mongoose from "mongoose";

interface DrawingInterface extends DrawingAttributes {
    actions?: ServerEditorAction[];
}

export type DrawingModel = mongoose.Document & DrawingInterface;

const drawingSchema = new mongoose.Schema({
    name: String,
    owner: { type: mongoose.Schema.Types.ObjectId, ref: "User" },
    actions: Array,
}, { timestamps: true });

const Drawing = mongoose.model("Drawing", drawingSchema);
export default Drawing;