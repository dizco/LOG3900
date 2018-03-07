import { DrawingAttributes } from "./drawing-attributes";
import { ServerEditorAction } from "../sockets/server-editor-action";
import * as mongoose from "mongoose";
import * as mongoosePaginate from "mongoose-paginate";
import * as bcrypt from "bcrypt-nodejs";
import { NextFunction } from "express";

interface DrawingInterface extends DrawingAttributes {
    actions?: ServerEditorAction[];
}

export type DrawingModel = mongoose.Document & DrawingInterface;

const drawingSchema = new mongoose.Schema({
    name: String,
    owner: { type: mongoose.Schema.Types.ObjectId, ref: "User" },
    protection: { active: { type: Boolean, default: false }, password: { type: String, default: "" } },
    actions: [{ actionId: Number, name: String, author: { type: mongoose.Schema.Types.ObjectId, ref: "User" }, timestamp: Number }],
}, { timestamps: true });
drawingSchema.plugin(mongoosePaginate);

/**
 * Password hash middleware.
 */
drawingSchema.pre("save", function save(next) {
    const drawing = this;
    if (!drawing.isModified("protection.password")) {
        return next();
    }
    hashPassword(drawing, next);
});
drawingSchema.pre("findOneAndUpdate", function findOneAndUpdate(next) {
    const drawing = this.getUpdate();
    drawing.id = this.getQuery()._id;
    if (!(drawing && drawing.protection && drawing.protection.password !== undefined)) { //protection has not been modified
        return next();
    }
    hashPassword(drawing, next);
});
function hashPassword(drawing: any, next: NextFunction) {
    if (!drawing.protection.active) { //Bypass password hashing if its not active
        if (process.env.NODE_ENV === "development") {
            console.log(`Bypassing drawing (id ${drawing.id}) password hashing because protection is inactive.`);
        }
        delete drawing.protection.password;
        return next();
    }
    bcrypt.genSalt(10, (err, salt) => {
        if (err) {
            return next(err);
        }
        bcrypt.hash(drawing.protection.password, salt, undefined, (err: mongoose.Error, hash) => {
            if (err) {
                return next(err);
            }
            drawing.protection.password = hash;
            next();
        });
    });
}

drawingSchema.methods.comparePassword = function(candidatePassword: string, cb: (err: any, isMatch: any) => {}) {
    bcrypt.compare(candidatePassword, this.protection.password, (err: mongoose.Error, isMatch: boolean) => {
        cb(err, isMatch);
    });
};

//Remove the protection password field
if (!(<any>drawingSchema).options.toObject) (<any>drawingSchema).options.toObject = {};
(<any>drawingSchema).options.toObject.transform = function (doc: any, ret: any, options: any) {
    delete ret.protection.password;
    if (ret.actions) {
        ret.actions = <any>ret.actions.map((action: any) => {
            action.id = action.actionId;
            delete action.actionId;
            delete action._id;
            return action;
        });
    }
    return ret;
};

const Drawing = mongoose.model<DrawingModel>("Drawing", drawingSchema);
export default Drawing;