import * as mongoose from "mongoose";
import { NextFunction, Request, Response } from "express";
import { default as Template, TemplateModel } from "../models/template";
import { PaginateResult } from "mongoose";
import { default as Drawing, DrawingModel } from "../models/drawings/drawing";
import { default as Like } from "../models/like";

const enum TemplateFields {
    DrawingId = "drawing-id",
}

/**
 * GET /templates
 */
export let getTemplates = (req: Request, res: Response, next: NextFunction) => {
    req.checkQuery("page", "Page number must be an integer above 0").isInt({ min: 1 });

    const errors = req.validationErrors();

    if (errors) {
        return res.status(422).json({ status: "error", error: "Validation errors.", hints: errors });
    }

    const options = {
        select: "name mode thumbnail",
        populate: [
            { path: "owner", select: "username" },
        ],
        page: req.query.page,
    };

    Template.paginate({}, options, (err: any, templates: PaginateResult<TemplateModel>) => {
        if (err) {
            return next(err);
        }

        return appendLikesToTemplates(req, res, next, templates);
    }).catch((reason => {
        console.log("Failed to fetch and paginate templates", reason);
        return next(reason);
    }));
};

function appendLikesToTemplates(req: Request, res: Response, next: NextFunction, templates: PaginateResult<TemplateModel>): void {
    const idList: any[] = [];
    templates.docs = <any>templates.docs.map(function(template) {
        idList.push(mongoose.Types.ObjectId(template.id));
        return template.toObject(); //Transform every template
    });

    const aggregateOptions = [
        {
            $match: { template: { $in: idList }},
        },
        {
            $lookup: //Join Users table
                {
                    from: "users",
                    localField: "user",
                    foreignField: "_id",
                    as: "users_docs"
                }
        },
        {
            $unwind: "$users_docs" //Transform "users_docs" from an array to individual documents
        },
        {
            $addFields: { //Don't know why, but the "group" pipeline can't find items if they're nested so here we put it at the root of the doc
                user_id: "$users_docs._id",
                user_username: "$users_docs.username",
            },
        },
        {
            $group: { //Group likes per template
                _id: "$template",
                users: { $push: { _id: "$user_id", username: "$user_username" }},
                count: { $sum: 1 },
            }
        }
    ];
    Like.aggregate(aggregateOptions).exec((err: any, result: any[]) => {
        console.log("likes", err, result, JSON.stringify(result));
        if (err) {
            return next(err);
        }
        templates.docs.forEach((template: any) => {
            const templateLikes = result.find((likes: any) => likes._id.toString() === template._id.toString());
            template.likes = {
                users: (templateLikes) ? templateLikes.users : [],
                total: (templateLikes) ? templateLikes.count : 0,
            };
        });

        return res.json(templates);
    });
}

/**
 * POST /templates
 */
export let postTemplate = (req: Request, res: Response, next: NextFunction) => {
    req.checkBody(TemplateFields.DrawingId, "Template drawing id cannot be empty").notEmpty();
    req.checkBody(TemplateFields.DrawingId, "Drawing id must be of type ObjectId").matches(/^[a-f\d]{24}$/i);

    const errors = req.validationErrors();

    if (errors) {
        return res.status(422).json({ status: "error", error: "Validation errors.", hints: errors });
    }

    const populateOptions = [
        { path: "owner" },
    ];
    Drawing.findOne({ _id: req.body[TemplateFields.DrawingId] }).populate(populateOptions).exec((err: any, drawing: DrawingModel) => {
        if (err) {
            return next(err);
        }
        if (!drawing) {
            return res.status(404).json({ status: "error", error: "Drawing not found." });
        }

        const template = new Template({
            name: drawing.name,
            mode: drawing.mode,
            owner: drawing.owner,
            strokes: drawing.strokes,
            pixels: drawing.pixels,
            thumbnail: drawing.thumbnail,
        });

        template.save((err) => {
            if (err) {
                return next(err);
            }
            return res.json({ status: "success", objectId: template.id });
        });
    });
};

/**
 * GET /templates/:id
 */
export let getTemplate = (req: Request, res: Response, next: NextFunction) => {
    req.checkParams("id", "Drawing id cannot be empty").notEmpty();
    req.checkParams("id", "Id must be of type ObjectId").matches(/^[a-f\d]{24}$/i);

    const errors = req.validationErrors();

    if (errors) {
        return res.status(422).json({ status: "error", error: "Failed to validate template id.", hints: errors });
    }

    const populateOptions = [
        { path: "owner", select: "username" },
    ];
    Template.findOne({_id: req.params.id}).populate(populateOptions).exec((err: any, template: TemplateModel) => {
        if (err) {
            return next(err);
        }
        if (!template) {
            return res.status(404).json({ status: "error", error: "Template not found." });
        }

        return res.json(template.toObject({ versionKey: false }));
    });
};

/**
 * POST /templates/:id/likes
 */
export let postTemplateLike = (req: Request, res: Response, next: NextFunction) => {
    req.checkParams("id", "Drawing id cannot be empty").notEmpty();
    req.checkParams("id", "Id must be of type ObjectId").matches(/^[a-f\d]{24}$/i);

    const errors = req.validationErrors();

    if (errors) {
        return res.status(422).json({ status: "error", error: "Failed to validate template id.", hints: errors });
    }

    Template.findOne({_id: req.params.id}, (err: any, template: TemplateModel) => {
        if (err) {
            return next(err);
        }
        if (!template) {
            return res.status(404).json({ status: "error", error: "Template not found." });
        }

        const like = new Like({
            template: template,
            user: req.user,
        });

        like.save((err) => {
            if (err) {
                if (err.name === "MongoError" && err.code === 11000) {
                    return res.status(418).json({ status: "error", error: "Template can't be liked twice." });
                }
                return next(err);
            }
            return res.json({ status: "success", objectId: like.id });
        });
    });
};
