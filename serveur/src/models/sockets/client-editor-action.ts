export interface ClientEditorAction {
    action: {
        id: number | string;
        name: string;
    };

    drawing: {
        id: number | string;
    };
}