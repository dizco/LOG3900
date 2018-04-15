using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
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

        public ObservableCollection<TemplateModel> FilteredTemplates { get; set; }
            = new ObservableCollection<TemplateModel>();

        public ObservableCollection<string> AutosavedDrawings { get; set; } = new ObservableCollection<string>();

        public ObservableCollection<TemplateModel> TemplateList { get; set; } =
            new ObservableCollection<TemplateModel>();

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///     Tuple contains DrawingId, DrawingName, EditingModeOption (Stroke/Pixel), StrokeList (StrokeModel), PixelList
        ///     (PixelModel)
        /// </summary>
        public event EventHandler<Tuple<string, string, EditingModeOption, List<StrokeModel>, List<PixelModel>>>
            NewDrawingCreated;

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

        internal async void LoadTemplateList()
        {
            TemplateList.Clear();

            int currentPage = 1;
            int maxPages = 0;
            int retryCount = 0;
            int maxRetries = 3;
            do
            {
                HttpResponseMessage response = await RestHandler.GetTemplates(currentPage);

                if (!response.IsSuccessStatusCode)
                {
                    retryCount++;
                    continue;
                }

                try
                {
                    JObject content = JObject.Parse(await response.Content.ReadAsStringAsync());
                    maxPages = content.GetValue("pages").ToObject<int>();
                    TemplateModel[] templateArray = content.GetValue("docs").ToObject<TemplateModel[]>();

                    foreach (TemplateModel template in templateArray)
                    {
                        TemplateList.Add(template);
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
            }
        }

        private async Task<List<StrokeModel>> GetStrokeTemplate(string templateId)
        {
            List<StrokeModel> loadedStrokeTemplate = null;

            if (templateId != null)
            {
                HttpResponseMessage response = await RestHandler.GetTemplate(templateId);
                HttpContent content = response.Content;
                JObject contentJson = JObject.Parse(await content.ReadAsStringAsync());
                loadedStrokeTemplate = contentJson["strokes"].ToObject<List<StrokeModel>>();
            }

            return loadedStrokeTemplate;
        }

        private async Task<List<PixelModel>> GetPixelTemplate(string templateId)
        {
            List<PixelModel> loadedPixelTemplate = null;
            if (templateId != null)
            {
                HttpResponseMessage response = await RestHandler.GetTemplate(templateId);
                HttpContent content = response.Content;
                JObject contentJson = JObject.Parse(await content.ReadAsStringAsync());
                loadedPixelTemplate = contentJson["strokes"].ToObject<List<PixelModel>>();
            }

            return loadedPixelTemplate;
        }

        internal void FilterTemplatesByEditingMode(string selectedEditingMode)
        {
            FilteredTemplates.Clear();
            selectedEditingMode = selectedEditingMode.Equals("Trait") ? "stroke" : "pixel";

            TemplateModel emptyTemplate = new TemplateModel
            {
                Name = "< Vide >"
            };
            FilteredTemplates.Add(emptyTemplate);

            foreach (TemplateModel template in TemplateList)
            {
                if (template.Mode.Equals(selectedEditingMode))
                {
                    FilteredTemplates.Add(template);
                }
            }
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

        internal async void CreateNewDrawing(string drawingName, EditingModeOption option, [Optional] string templateId,
            [Optional] string password, bool visibilityPublic = true)
        {
            if (await RestHandler.ValidateServerUri())
            {
                HttpResponseMessage response;
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

                        List<StrokeModel> loadedStrokeTemplate = null;
                        List<PixelModel> loadedPixelTemplate = null;

                        if (option == EditingModeOption.Trait)
                        {
                            loadedStrokeTemplate = await GetStrokeTemplate(templateId);
                        }
                        else if (option == EditingModeOption.Pixel)
                        {
                            loadedPixelTemplate = await GetPixelTemplate(templateId);
                        }

                        OnNewDrawingCreated(new Tuple<string, string, EditingModeOption, List<StrokeModel>,
                                                List<PixelModel>>(drawingId, drawingName,
                                                                  option, loadedStrokeTemplate, loadedPixelTemplate));
                    }
                    catch
                    {
                        UserAlerts.ShowInfoMessage("Le dessin sera créé en mode hors ligne");
                        CreateNewOfflineDrawing(drawingName, option);
                    }
                }
                else if (response.StatusCode == (HttpStatusCode) 422)
                {
                    try
                    {
                        string hintMessages = await ServerErrorParser.ParseHints(response);

                        UserAlerts.ShowErrorMessage(hintMessages);
                    }
                    catch (Exception e)
                    {
                        UserAlerts.ShowErrorMessage("Le dessin sera créé en mode hors ligne" + "\n" + e.Message);
                        CreateNewOfflineDrawing(drawingName, option);
                    }
                }
                else
                {
                    UserAlerts.ShowInfoMessage("Le dessin sera créé en mode hors ligne");
                    CreateNewOfflineDrawing(drawingName, option);
                }
            }
            else
            {
                UserAlerts.ShowInfoMessage("Le dessin sera créé en mode hors ligne");
                CreateNewOfflineDrawing(drawingName, option);
            }
        }

        private void OnNewDrawingCreated(
            Tuple<string, string, EditingModeOption, List<StrokeModel>, List<PixelModel>> drawingParams)
        {
            NewDrawingCreated?.Invoke(this, drawingParams);
        }

        private void CreateNewOfflineDrawing(string drawingName, EditingModeOption option)
        {
            OnNewDrawingCreated(new Tuple<string, string, EditingModeOption, List<StrokeModel>, List<PixelModel>>
                                    (string.Empty, drawingName, option, null, null));
        }

        /// <summary>
        ///     Parses server response and display appropriate error message
        /// </summary>
        /// <param name="response">Serialized JSON response</param>
        internal static void OnResponseError(string response)
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
