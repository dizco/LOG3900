import { Request, Response } from "express";
import { Drawing } from "../models/drawings/drawing";
import { Author } from "../models/author";
import { Stroke } from "../models/drawings/stroke";
import { Dot } from "../models/drawings/dot";

/**
 * POST /drawings
 */
export let postDrawing = (req: Request, res: Response) => {
    //TODO: Assert that every mandatory field is filled out
    //TODO: Check cast to Drawing
    //TODO: Save new database entry with drawing properties and strokes
    //TODO: Send back json status
    res.status(500).json({
        message: "POST not implemented yet",
    });
};

/**
 * GET /drawings/:id
 */
export let getDrawing = (req: Request, res: Response) => {
    //TODO: Fetch drawing by id in database and cast to Drawing
    //TODO: Verify if drawing exists. If not, return error

    const author: Author = {
        id: 134,
        username: "dizco",
        name: "Gabriel",
        url: "https://example.com/users/dizco",
        avatar_url: "https://example.com/users/dizco/avatar.jpg",
    };
    const dots: Dot[] = [{color: "#fff", x: 1, y: 2}];
    const strokes: Stroke[] = [{
        author: { //We don't need to send the whole author information every time
            id: 134
        },
        dots: dots,
    }];
    const drawing: Drawing = {
        id: req.params.id,
        name: "This is the way",
        owner: author,
        strokes: strokes,
    };

    res.json(drawing);
};

/**
 * PUT /drawings/:id
 */
export let putDrawing = (req: Request, res: Response) => {
    res.status(500).json({
        message: "PUT not implemented yet",
    });
};