using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace PolyPaint.Utilitaires
{
    public static class RestHandler
    {
        public static HttpClientHandler Handler = new HttpClientHandler();
        private static readonly HttpClient Client = new HttpClient(Handler);
        public static string ServerUri { get; set; }

        public static async Task<bool> ValidateServerUri()
        {
            try
            {
                await Client.GetAsync(ServerUri);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static async Task<HttpResponseMessage> LoginInfo(string username, string password)
        {
            Dictionary<string, string> userInfo = new Dictionary<string, string>
            {
                {"email", username},
                {"password", password}
            };

            return await Client.PostAsync(ServerUri + "/login", new FormUrlEncodedContent(userInfo));
        }

        public static async Task<HttpResponseMessage> RegisterInfo(string username, string password)
        {
            Dictionary<string, string> userInfo = new Dictionary<string, string>
            {
                {"email", username},
                {"password", password}
            };
            return await Client.PostAsync(ServerUri + "/register", new FormUrlEncodedContent(userInfo));
        }
    }
}