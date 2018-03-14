using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using PolyPaint.Helpers;
using PolyPaint.Helpers.Communication;
using PolyPaint.Models.ApiModels;
using PolyPaint.Models.MessagingModels;

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

        // TODO: Modify this function once server saving protocol is established
        /// <summary>
        ///     Tuple contains DrawingId, DrawingName, EditingModeOption (Stroke/Pixel) and actions to replay
        /// </summary>
        public event EventHandler<Tuple<string, string, EditingModeOption, List<EditorActionModel>>>
            OnlineDrawingJoined;

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
            EditorStroke.FetchAutosavedDrawings()?.ToList()
                  .ForEach(drawing => AutosavedDrawings.Add(EditorStroke.AutosaveFileNameToString(drawing)));
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

        internal async void CreateNewDrawing(string drawingName, EditingModeOption option)
        {
            if (await RestHandler.ValidateServerUri())
            {
                HttpResponseMessage response = await RestHandler.CreateDrawing(drawingName, option);

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
                        UserAlerts.ShowErrorMessage("Le dessin sera créer en mode hors ligne");
                        CreateNewOfflineDrawing(drawingName, option);
                    }
                }
                else
                {
                    CreateNewOfflineDrawing(drawingName, option);
                }
            }
            else
            {
                CreateNewOfflineDrawing(drawingName, option);
            }
        }

        internal async void JoinOnlineDrawing(string id)
        {
            HttpResponseMessage response = await RestHandler.GetOnlineDrawing(id);
            if (response.IsSuccessStatusCode)
            {
                try
                {
                    JObject content = JObject.Parse(await response.Content.ReadAsStringAsync());

                    // TODO: Modify this function once server saving protocol is established
                    List<EditorActionModel> editorActionHistory = null;
                    string drawingName = content.GetValue("name").ToString();

                    // TODO: Get actual editing mode from drawing info once implemented
                    EditingModeOption option = EditingModeOption.Trait;

                    OnOnlineDrawingJoined(id, drawingName, option, editorActionHistory);
                }
                catch
                {
                    UserAlerts.ShowErrorMessage("Une erreure est survenue");
                }
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
            List<EditorActionModel> actions)
        {
            Tuple<string, string, EditingModeOption, List<EditorActionModel>> drawingParams =
                new Tuple<string, string, EditingModeOption, List<EditorActionModel>>(drawingId, drawingName, option,
                                                                                      actions);

            OnlineDrawingJoined?.Invoke(this, drawingParams);
        }
    }
}
