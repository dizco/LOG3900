using Newtonsoft.Json;

namespace PolyPaint.Models.ApiModels
{
    internal class OnlineDrawingModel
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "_id")]
        public string Id { get; set; }
    }

    public enum EditingModeOption
    {
        Trait,
        Pixel
    }
}
