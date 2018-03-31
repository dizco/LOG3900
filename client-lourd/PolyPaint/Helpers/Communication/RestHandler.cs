﻿using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PolyPaint.CustomComponents;
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

        public static async Task<HttpResponseMessage> GetOnlineDrawingsForAuthor(string userEmail, int page)
        {
            return await Client.GetAsync($"{ServerUri}/drawings?owner={userEmail}&page={page}");
        }

        public static async Task<HttpResponseMessage> GetPublicOnlineDrawings(int page)
        {
            return await Client.GetAsync($"{ServerUri}/drawings?visibility=public&page={page}");
        }

        public static async Task<HttpResponseMessage> CreateDrawing(string drawingName, EditingModeOption option,
            bool visibilityPublic = true)
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
                {"visibility", visibilityPublic ? "public" : "private"},
                {"mode", mode}
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
                {"visibility", visibilityPublic ? "public" : "private"},
                {"mode", mode}
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

        public static async Task<HttpResponseMessage> GetDrawingActionsHistory(string drawingId, int page = 1)
        {
            return await Client.GetAsync($"{ServerUri}/drawings/{drawingId}/actions?page={page}");
        }

        public static async Task<HttpResponseMessage> UpdateDrawingProtection(string drawingId,
            [Optional] string password)
        {
            Dictionary<string, string> requestContent = new Dictionary<string, string>();

            if (password != null)
            {
                requestContent.Add("protection-active", "true");
                requestContent.Add("protection-password", password);
            }
            else
            {
                requestContent.Add("protection-active", "false");
            }

            return await Client.PatchAsync($"{ServerUri}/drawings/{drawingId}",
                                           new FormUrlEncodedContent(requestContent));
        }

        public static async Task<HttpResponseMessage> UpdateDrawingVisibility(string drawingId, bool makePublic)
        {
            Dictionary<string, string> requestContent = new Dictionary<string, string>
            {
                {"visibility", makePublic ? "public" : "private"}
            };

            return await Client.PatchAsync($"{ServerUri}/drawings/{drawingId}",
                                           new FormUrlEncodedContent(requestContent));
        }

        public static async Task<HttpResponseMessage> UpdateDrawingThumbnail(string drawingId, string base64Bitmap)
        {
            Dictionary<string, string> content = new Dictionary<string, string>
            {
                {"thumbnail", base64Bitmap}
            };

            string jsonpayload = JsonConvert.SerializeObject(content);

            return await Client.PutAsync($"{ServerUri}/drawings/{drawingId}/thumbnail",
                                         new StringContent(jsonpayload, Encoding.UTF8, "application/json"));
        }

        public static async Task<HttpResponseMessage> GetDrawingThumbnail(string drawingId)
        {
            return await Client.GetAsync($"{ServerUri}/drawings/{drawingId}/thumbnail");
        }
    }
}
