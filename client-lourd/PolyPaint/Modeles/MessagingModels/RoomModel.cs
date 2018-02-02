using Newtonsoft.Json;

namespace PolyPaint.Modeles.MessagingModels
{
    public class RoomModel
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; } = null;

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; } = null;
    }
}