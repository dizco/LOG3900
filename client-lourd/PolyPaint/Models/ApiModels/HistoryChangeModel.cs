using System;
using Newtonsoft.Json;
using PolyPaint.Models.MessagingModels;

namespace PolyPaint.Models.ApiModels
{
    class HistoryChangeModel : MessageModelBase
    {
        [JsonProperty(PropertyName = "id")]
        public int ActionId { get; set; }

        [JsonProperty(PropertyName = "timestamp")]
        public long Timestamp { get; set; }

        [JsonProperty(PropertyName = "author")]
        public AuthorModel Author { get; set; }
    }

    internal class HistoryDisplayModel
    {
        public string ActionDescription { get; set; }
        public DateTime Timestamp { get; set; }
        public string Author { get; set; }
    }
}
