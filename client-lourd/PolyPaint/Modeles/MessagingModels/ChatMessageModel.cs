using Newtonsoft.Json;

namespace PolyPaint.Modeles.MessagingModels
{
    public class ChatMessageModel : MessageModelBase
    {
        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }

        [JsonProperty(PropertyName = "author")]
        public AuthorModel Author { get; set; }

        [JsonProperty(PropertyName = "room")]
        public RoomModel Room { get; set; }

        [JsonProperty(PropertyName = "timestamp")]
        public int Timestamp { get; set; } = 0;
    }
}