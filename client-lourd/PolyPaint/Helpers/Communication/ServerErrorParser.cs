using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace PolyPaint.Helpers.Communication
{
    internal static class ServerErrorParser
    {
        internal static async Task<string> ParseHints(HttpResponseMessage response)
        {
            JObject content = JObject.Parse(await response.Content.ReadAsStringAsync());

            List<Dictionary<string, object>> hints =
                content.GetValue("hints").ToObject<List<Dictionary<string, object>>>();

            string hintMessages = null;
            foreach (Dictionary<string, object> hint in hints)
            {
                hintMessages += hint["msg"];
                if (hint != hints.Last())
                {
                    hintMessages += "\n";
                }
            }

            return hintMessages;
        }
    }
}
