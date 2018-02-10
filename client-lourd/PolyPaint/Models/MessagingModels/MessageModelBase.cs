using Newtonsoft.Json;

namespace PolyPaint.Models.MessagingModels
{
    public class MessageModelBase
    {
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }
    }
}