using Newtonsoft.Json;

namespace PolyPaint.Models.MessagingModels
{
    public class RoomModel
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
    }
}