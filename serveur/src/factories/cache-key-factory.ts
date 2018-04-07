export class CacheKeyFactory {
    private static readonly DRAWINGS_KEY = "drawings";

    public static createDrawingCacheKey(drawingId: string): string {
        return CacheKeyFactory.DRAWINGS_KEY + "." + drawingId;
    }
}
