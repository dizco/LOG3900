export interface Action {
    id: number | string;
    name: string;
}

export function IsAction(action: any): action is Action {
    action = <Action>action;
    if (!("id" in action)) {
        return false;
    }
    else if (!("name" in action)) {
        return false;
    }

    return true;
}
