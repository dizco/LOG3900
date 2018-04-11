using System.Collections.Generic;
using System.Windows.Documents;
using Newtonsoft.Json;

namespace PolyPaint.Models.MessagingModels
{
    public class LikesModel
    {
        //TODO: users JsonProperty

        [JsonProperty(PropertyName = "total")]
        public int Total { get; set; }
    }
}
