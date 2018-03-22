using System;
using Newtonsoft.Json;

namespace PolyPaint.Models.MessagingModels
{
    class HistoryChangeModel : MessageModelBase
    {
        [JsonProperty(PropertyName = "change")]
        public string Change { get; set; }

        [JsonProperty(PropertyName = "author")]
        public AuthorModel Author { get; set; }

        [JsonProperty(PropertyName = "room")]
        public RoomModel Room { get; set; }

        [JsonProperty(PropertyName = "timestamp")]
        public long Timestamp { get; set; }
    }

    internal class HistoryChangeDisplayModel
    {
        public string ChangeText { get; set; }
        public DateTime Timestamp { get; set; }
        public string AuthorName { get; set; }
    }
}
