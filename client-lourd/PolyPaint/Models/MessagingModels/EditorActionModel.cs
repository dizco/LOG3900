using Newtonsoft.Json;

namespace PolyPaint.Models.MessagingModels
{
    public class EditorActionModel : MessageModelBase
    {
        [JsonProperty(PropertyName = "action")]
        public StrokeActionModel Action { get; set; }

        [JsonProperty(PropertyName = "author")]
        public AuthorModel Author { get; set; }

        [JsonProperty(PropertyName = "drawing")]
        public DrawingModel Drawing { get; set; }

        [JsonProperty(PropertyName = "delta")]
        public DeltaModel Delta { get; set; }

        [JsonProperty(PropertyName = "layer")]
        public int Layer { get; set; }
    }

    public class DeltaModel
    {
        [JsonProperty(PropertyName = "add")]
        public StrokeModel[] Add { get; set; }

        [JsonProperty(PropertyName = "remove")]
        public string[] Remove { get; set; }
    }

    public class StrokeActionModel
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
    }

    public enum ActionIds
    {
        NewStroke = 1,
        ReplaceStroke
    }

    public class DrawingModel
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
    }

    public class StrokeModel
    {
        [JsonProperty(PropertyName = "strokeUuid")]
        public string Uuid { get; set; }

        [JsonProperty(PropertyName = "strokeAttributes")]
        public DrawingAttributesModel DrawingAttributes { get; set; }

        [JsonProperty(PropertyName = "dots")]
        public StylusPointModel[] Dots { get; set; }
    }

    public class DrawingAttributesModel
    {
        [JsonProperty(PropertyName = "color")]
        public string Color { get; set; }

        [JsonProperty(PropertyName = "height")]
        public double Height { get; set; }

        [JsonProperty(PropertyName = "width")]
        public double Width { get; set; }

        [JsonProperty(PropertyName = "stylusTip")]
        public string StylusTip { get; set; }
    }

    public class StylusPointModel
    {
        [JsonProperty(PropertyName = "x")]
        public double X { get; set; }
        [JsonProperty(PropertyName = "y")]
        public double Y { get; set; }
    }
}
