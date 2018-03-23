using Newtonsoft.Json;

namespace PolyPaint.Models.MessagingModels
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
}
