import * as mongoose from "mongoose";
import { UserAttributes } from "./user-attributes";
import { TemplateInterface } from "./template";

interface LikeInterface {
    id: number | string;
    template: TemplateInterface;
    user: UserAttributes;
}

export type LikeModel = mongoose.Document & LikeInterface;

const likeSchema = new mongoose.Schema({
    template: { type: mongoose.Schema.Types.ObjectId, ref: "Template", required: true },
    user: { type: mongoose.Schema.Types.ObjectId, ref: "User", required: true },
}, { timestamps: true });

const Like = mongoose.model<LikeModel>("Like", likeSchema);
export default Like;
