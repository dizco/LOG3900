import * as mongoose from "mongoose";
import * as mongoosePaginate from "mongoose-paginate";
import { Author } from "../sockets/author";

interface ActionInterface {
    actionId: number;
    name: string;
    author: Author;
    timestamp: number;
}

export type ActionModel = mongoose.Document & ActionInterface;

const actionSchema = new mongoose.Schema({
    drawing: { type: mongoose.Schema.Types.ObjectId, ref: "Drawing" },
    actionId: Number,
    name: String,
    author: { type: mongoose.Schema.Types.ObjectId, ref: "User" },
    timestamp: Number,
});
actionSchema.plugin(mongoosePaginate);

//Cleanup
if (!(<any>actionSchema).options.toObject) (<any>actionSchema).options.toObject = {};
(<any>actionSchema).options.toObject.transform = function (doc: any, ret: any, options: any) {
    ret.id = ret.actionId;
    delete ret.actionId;
    delete ret._id;

    return ret;
};

const Action = mongoose.model<ActionModel>("Action", actionSchema);
export default Action;