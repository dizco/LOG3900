using Newtonsoft.Json;

namespace PolyPaint.Models.MessagingModels
{
    public class PixelEditorActionModel : MessageModelBase
    {
        [JsonProperty(PropertyName = "action")]
        public PixelActionModel Action { get; set; }

        [JsonProperty(PropertyName = "author")]
        public AuthorModel Author { get; set; }

        [JsonProperty(PropertyName = "drawing")]
        public DrawingModel Drawing { get; set; }

        [JsonProperty(PropertyName = "pixels")]
        public PixelsModel Pixels { get; set; }
    }

    public class PixelActionModel
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
    }

    public enum PixelActionIds
    {
        NewPixels = 1,
        Lock,
        Unlock
    }

    public class PixelsModel
    {
        [JsonProperty(PropertyName = "pixels")]
        public PixelModel[] Pixels { get; set; }
    }

    public class PixelModel
    {
        [JsonProperty(PropertyName = "x")]
        public double X { get; set; }

        [JsonProperty(PropertyName = "y")]
        public double Y { get; set; }

        [JsonProperty(PropertyName = "color")]
        public string Color { get; set; }
    }
}
