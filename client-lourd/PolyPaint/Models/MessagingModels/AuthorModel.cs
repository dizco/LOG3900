using Newtonsoft.Json;

namespace PolyPaint.Models.MessagingModels
{
    public class AuthorModel
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "username")]
        public string Username { get; set; }

        [JsonProperty(PropertyName = "url")]
        public string Url { get; set; }
    }
}