using System;
using Newtonsoft.Json;

namespace PolyPaint.Models.MessagingModels
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
        public long Timestamp { get; set; }
    }

    internal class ChatMessageDisplayModel
    {
        public string MessageText { get; set; }
        public DateTime Timestamp { get; set; }
        public string AuthorName { get; set; }
    }
}