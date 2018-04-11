using Newtonsoft.Json;
using PolyPaint.Models.MessagingModels;

namespace PolyPaint.Models.ApiModels
{
    internal class OnlineDrawingModel
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "_id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "protection")]
        public ProtectionModel Protection { get; set; }

        [JsonProperty(PropertyName = "owner")]
        public AuthorModel Owner { get; set; }

        [JsonProperty(PropertyName = "visibility")]
        public string Visibility { get; set; }
    }

    internal class ProtectionModel
    {
        [JsonProperty(PropertyName = "active")]
        public bool Active { get; set; }
    }

    internal class TemplateModel
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "_id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "owner")]
        public AuthorModel Owner { get; set; }

        [JsonProperty(PropertyName = "likes")]
        public LikesModel Likes { get; set; }
    }

    public enum EditingModeOption
    {
        Trait,
        Pixel
    }
}
