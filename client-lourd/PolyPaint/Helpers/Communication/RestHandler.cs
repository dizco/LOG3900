﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace PolyPaint.Helpers.Communication
{
    public static class RestHandler
    {
        public static HttpClientHandler Handler = new HttpClientHandler();
        private static readonly HttpClient Client = new HttpClient(Handler);
        public static string ServerUri { get; set; }

        /// <summary>
        ///     This method makes a GET request to the server URI specified by the user. If the request throws an exception, the
        ///     server does not exist at the specified adress.
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> ValidateServerUri()
        {
            try
            {
                HttpResponseMessage response = await Client.GetAsync(ServerUri + "/ping");
                if (response.IsSuccessStatusCode) return true;
            }
            catch
            {
                return false;
            }

            return false;
        }

        public static async Task<HttpResponseMessage> LoginInfo(string username, string password)
        {
            Dictionary<string, string> userInfo = new Dictionary<string, string>
            {
                {"email", username},
                {"password", password}
            };

            return await Client.PostAsync($"{ServerUri}/login", new FormUrlEncodedContent(userInfo));
        }

        public static async Task<HttpResponseMessage> RegisterInfo(string username, string password)
        {
            Dictionary<string, string> userInfo = new Dictionary<string, string>
            {
                {"email", username},
                {"password", password}
            };
            return await Client.PostAsync($"{ServerUri}/register", new FormUrlEncodedContent(userInfo));
        }

        public static async Task<HttpResponseMessage> AllDrawings(int page)
        {
            return await Client.GetAsync($"{ServerUri}/drawings?page={page}");
        }
    }
}