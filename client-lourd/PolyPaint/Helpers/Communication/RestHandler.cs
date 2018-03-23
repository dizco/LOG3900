﻿using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using PolyPaint.Models.ApiModels;

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
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }

            return false;
        }

        public static async Task<HttpResponseMessage> LoginUser(string username, string password)
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

        public static async Task<HttpResponseMessage> GetOnlineDrawingsForPage(int page)
        {
            return await Client.GetAsync($"{ServerUri}/drawings?page={page}");
        }

        public static async Task<HttpResponseMessage> CreateDrawing(string drawingName, EditingModeOption option, bool visibilityPublic = true)
        {
            string mode = null;
            if (option == EditingModeOption.Pixel)
            {
                mode = "pixel";
            }
            else if (option == EditingModeOption.Trait)
            {
                mode = "stroke";
            }

            Dictionary<string, string> drawingInfo = new Dictionary<string, string>
            {
                {"name", drawingName},
                {"visibility", visibilityPublic? "public":"private"},
                {"mode",mode}
            };

            return await Client.PostAsync($"{ServerUri}/drawings", new FormUrlEncodedContent(drawingInfo));
        }

        public static async Task<HttpResponseMessage> CreateDrawing(string drawingName, EditingModeOption option,
            string password, bool visibilityPublic = true)
        {
            string mode = null;
            if (option == EditingModeOption.Pixel)
            {
                mode = "pixel";
            }
            else if (option == EditingModeOption.Trait)
            {
                mode = "stroke";
            }

            Dictionary<string, string> drawingInfo = new Dictionary<string, string>
            {
                {"name", drawingName},
                {"protection-active", "true"},
                {"protection-password", password},
                {"visibility", visibilityPublic? "public":"private"},
                {"mode",mode}
            };

            return await Client.PostAsync($"{ServerUri}/drawings", new FormUrlEncodedContent(drawingInfo));
        }

        public static async Task<HttpResponseMessage> GetOnlineDrawing(string drawingId)
        {
            return await Client.GetAsync($"{ServerUri}/drawings/{drawingId}");
        }

        public static async Task<HttpResponseMessage> GetOnlineDrawing(string drawingId, string password)
        {
            Client.DefaultRequestHeaders.Add("protection-password", password);

            Task<HttpResponseMessage> response = Client.GetAsync($"{ServerUri}/drawings/{drawingId}");

            Client.DefaultRequestHeaders.Remove("protection-password");

            return await response;
        }
    }
}
