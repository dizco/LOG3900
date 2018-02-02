using Newtonsoft.Json;

namespace PolyPaint.Modeles.MessagingModels
{
    public class AuthorModel
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; } = 0;

        [JsonProperty(PropertyName = "username")]
        public string Username { get; set; } = null;

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; } = null;

        [JsonProperty(PropertyName = "url")]
        public string Url { get; set; } = null;

        [JsonProperty(PropertyName = "avatar_url")]
        public string AvatarUrl { get; set; } = null;
    }
}