using Newtonsoft.Json;

namespace PolyPaint.Modeles.MessagingModels
{
    public class MessageModelBase
    {
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }
    }
}