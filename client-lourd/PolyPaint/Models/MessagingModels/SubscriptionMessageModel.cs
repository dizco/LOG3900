using System.ComponentModel;
using Newtonsoft.Json;

namespace PolyPaint.Models.MessagingModels
{
    public enum SubscriptionAction
    {
        [Description("join")] Join,
        [Description("leave")] Leave
    }

    internal class SubscriptionMessageModel : MessageModelBase
    {
        [JsonProperty(PropertyName = "drawing")]
        public DrawingModel Drawing { get; set; }

        [JsonProperty(PropertyName = "action")]
        public SubscribeAction Action { get; set; }

        public class SubscribeAction
        {
            [JsonProperty(PropertyName = "id")]
            public string Id { get; set; }

            [JsonProperty(PropertyName = "name")]
            public string Name { get; set; } = string.Empty;
        }
    }
}
