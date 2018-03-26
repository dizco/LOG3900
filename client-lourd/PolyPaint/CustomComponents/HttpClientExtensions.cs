using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace PolyPaint.CustomComponents
{
    internal static class HttpClientExtensions
    {
        /// <summary>
        ///     Send a PATCH request to the specified Uri as asynchronous operation
        /// </summary>
        public static async Task<HttpResponseMessage> PatchAsync(this HttpClient client, string requestUri,
            HttpContent content)
        {
            return await client.PatchAsync(new Uri(requestUri), content);
        }

        /// <summary>
        ///     Send a PATCH request to the specified Uri as asynchronous operation
        ///     Based off of https://stackoverflow.com/a/29772349
        /// </summary>
        public static async Task<HttpResponseMessage> PatchAsync(this HttpClient client, Uri requestUri,
            HttpContent content)
        {
            HttpMethod requestMethod = new HttpMethod("PATCH");
            HttpRequestMessage requestMessage = new HttpRequestMessage(requestMethod, requestUri)
            {
                Content = content
            };

            HttpResponseMessage response = new HttpResponseMessage();

            try
            {
                response = await client.SendAsync(requestMessage);
            }
            catch (TaskCanceledException)
            {
                // ignored
            }

            return response;
        }
    }
}
