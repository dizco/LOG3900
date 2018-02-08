using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace PolyPaint.Utilitaires
{
    public static class RestHandler
    {
        private static HttpClient _client = new HttpClient();
        public static string ServerUri { get; set; }

        public static async Task<HttpResponseMessage> LoginInfo(string username, string password)
        {
            Dictionary<string, string> userInfo = new Dictionary<string, string>
            {
                {"email", username},
                {"password", password}
            };

            return await _client.PostAsync(ServerUri + "/login", new FormUrlEncodedContent(userInfo));
        }
    }
}
