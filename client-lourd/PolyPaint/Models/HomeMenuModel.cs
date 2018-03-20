using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using Newtonsoft.Json.Linq;
using PolyPaint.Helpers;
using PolyPaint.Helpers.Communication;
using PolyPaint.Models.ApiModels;
using PolyPaint.Models.MessagingModels;
using PolyPaint.Views;

namespace PolyPaint.Models
{
    internal class HomeMenuModel : INotifyPropertyChanged
    {
        private List<OnlineDrawingModel> OnlineDrawingList { get; } =
            new List<OnlineDrawingModel>();

        public ObservableCollection<OnlineDrawingModel> FilteredDrawings { get; } =
            new ObservableCollection<OnlineDrawingModel>();

        public ObservableCollection<string> AutosavedDrawings { get; set; } = new ObservableCollection<string>();

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///     Tuple contains DrawingId, DrawingName and EditingModeOption (Stroke/Pixel)
        /// </summary>
        public event EventHandler<Tuple<string, string, EditingModeOption>> NewDrawingCreated;

        /// <summary>
        ///     Tuple contains DrawingId, DrawingName, EditingModeOption (Stroke/Pixel) and strokes to replay
        /// </summary>
        public event EventHandler<Tuple<string, string, EditingModeOption, List<StrokeModel>>>
            OnlineDrawingJoined;

        public event EventHandler<string> OnlineDrawingJoinFailed;

        internal async void LoadOnlineDrawingsList()
        {
            OnlineDrawingList.Clear();
            FilteredDrawings.Clear();

            int currentPage = 1;
            int maxPages = 0;
            const int maxRetries = 3;
            int retryCount = 0;
            do
            {
                HttpResponseMessage response = await RestHandler.GetOnlineDrawingsForPage(currentPage);
                if (!response.IsSuccessStatusCode)
                {
                    retryCount++;
                    continue;
                }

                try
                {
                    JObject content = JObject.Parse(await response.Content.ReadAsStringAsync());
                    maxPages = content.GetValue("pages").ToObject<int>();
                    OnlineDrawingModel[] docsArray =
                        content.GetValue("docs").ToObject<OnlineDrawingModel[]>();
                    foreach (OnlineDrawingModel drawing in docsArray)
                    {
                        OnlineDrawingList.Add(drawing);
                    }
                }
                catch
                {
                    retryCount++;
                    continue;
                }

                currentPage++;
                retryCount = 0;
            } while (currentPage <= maxPages && retryCount < maxRetries);

            if (retryCount >= maxRetries)
            {
                return;
            }

            foreach (OnlineDrawingModel drawing in OnlineDrawingList)
            {
                FilteredDrawings.Add(drawing);
            }
        }

        internal void LoadAutosavedDrawingsList()
        {
            AutosavedDrawings.Clear();
            StrokeEditor.FetchAutosavedDrawings()?.ToList()
                        .ForEach(drawing => AutosavedDrawings.Add(StrokeEditor.AutosaveFileNameToString(drawing)));
        }

        internal void SearchTextChangedHandlers(string keyword)
        {
            FilteredDrawings.Clear();
            foreach (OnlineDrawingModel drawing in OnlineDrawingList)
            {
                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    if (drawing.Name.ToLower().Contains(keyword) || drawing.Id.ToLower().Contains(keyword))
                    {
                        FilteredDrawings.Add(drawing);
                    }
                }
                else
                {
                    FilteredDrawings.Add(drawing);
                }
            }
        }

        internal async void CreateNewDrawing(string drawingName, EditingModeOption option, [Optional] string password,
            bool visibilityPublic = true)
        {
            if (await RestHandler.ValidateServerUri())
            {
                HttpResponseMessage response = null;
                if (password != null)
                {
                    response = await RestHandler.CreateDrawing(drawingName, option, password, visibilityPublic);
                }
                else
                {
                    response = await RestHandler.CreateDrawing(drawingName, option, visibilityPublic);
                }

                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        JObject content = JObject.Parse(await response.Content.ReadAsStringAsync());
                        string drawingId = content.GetValue("objectId").ToString();
                        OnNewDrawingCreated(new Tuple<string, string, EditingModeOption>(drawingId, drawingName,
                                                                                         option));
                    }
                    catch
                    {
                        UserAlerts.ShowErrorMessage("Le dessin sera créé en mode hors ligne");
                        CreateNewOfflineDrawing(drawingName, option);
                    }
                }
                else if (response.StatusCode == (HttpStatusCode) 422)
                {
                    try
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

                        UserAlerts.ShowErrorMessage(hintMessages);
                    }
                    catch
                    {
                        UserAlerts.ShowErrorMessage("Le dessin sera créé en mode hors ligne");
                        CreateNewOfflineDrawing(drawingName, option);
                    }
                }
                else
                {
                    UserAlerts.ShowErrorMessage("Le dessin sera créé en mode hors ligne");
                    CreateNewOfflineDrawing(drawingName, option);
                }
            }
            else
            {
                UserAlerts.ShowErrorMessage("Le dessin sera créé en mode hors ligne");
                CreateNewOfflineDrawing(drawingName, option);
            }
        }

        internal async void JoinOnlineDrawing(string id, bool isPasswordProtected)
        {
            HttpResponseMessage response;

            if (isPasswordProtected)
            {
                string drawingPassword = null;

                PasswordPrompt passwordPrompt = new PasswordPrompt();

                passwordPrompt.PasswordEntered += (s, password) =>
                {
                    drawingPassword = password;
                    passwordPrompt.Close();
                };

                passwordPrompt.ShowDialog();

                if (drawingPassword != null)
                {
                    response = await RestHandler.GetOnlineDrawing(id, drawingPassword);
                }
                else
                {
                    return;
                }
            }
            else
            {
                response = await RestHandler.GetOnlineDrawing(id);
            }

            if (response.IsSuccessStatusCode)
            {
                try
                {
                    JObject content = JObject.Parse(await response.Content.ReadAsStringAsync());

                    Dictionary<string, int> users = content.GetValue("users").ToObject<Dictionary<string, int>>();

                    users.TryGetValue("active", out int activeUsers);
                    users.TryGetValue("limit", out int limitUsers);
                    if (activeUsers >= limitUsers)
                    {
                        OnOnlineDrawingJoinFailed($"Impossible d\'ouvrir le dessin. Le nombre maximal d\'éditeurs a été atteint.");
                        return;
                    }
                    
                    List<StrokeModel> strokes = content.GetValue("strokes").ToObject<List<StrokeModel>>();
                    string drawingName = content.GetValue("name").ToString();

                    // TODO: Get actual editing mode from drawing info once implemented
                    EditingModeOption option = EditingModeOption.Trait;

                    OnOnlineDrawingJoined(id, drawingName, option, strokes);
                }
                catch
                {
                    UserAlerts.ShowErrorMessage("Une erreur est survenue");
                }
            }
            else
            {
                OnResponseError(await response?.Content.ReadAsStringAsync());
            }
        }

        private void OnNewDrawingCreated(Tuple<string, string, EditingModeOption> drawingParams)
        {
            NewDrawingCreated?.Invoke(this, drawingParams);
        }

        private void CreateNewOfflineDrawing(string drawingName, EditingModeOption option)
        {
            OnNewDrawingCreated(new Tuple<string, string, EditingModeOption>(string.Empty, drawingName, option));
        }

        private void OnOnlineDrawingJoined(string drawingId, string drawingName, EditingModeOption option,
            List<StrokeModel> strokes)
        {
            Tuple<string, string, EditingModeOption, List<StrokeModel>> drawingParams =
                new Tuple<string, string, EditingModeOption, List<StrokeModel>>(drawingId, drawingName, option,
                                                                                strokes);

            OnlineDrawingJoined?.Invoke(this, drawingParams);
        }

        private void OnOnlineDrawingJoinFailed(string error)
        {
            OnlineDrawingJoinFailed?.Invoke(this, error);
        }

        /// <summary>
        ///     Parses server response and display appropriate error message
        /// </summary>
        /// <param name="response">Serialized JSON response</param>
        private void OnResponseError(string response)
        {
            JObject responseJson;
            try
            {
                responseJson = JObject.Parse(response);
            }
            catch (Exception e)
            {
                UserAlerts.ShowErrorMessage(e.Message);
                return;
            }

            UserAlerts.ShowErrorMessage(responseJson?.GetValue("error")?.ToString() ?? "Une erreur est survenue");
        }
    }
}
