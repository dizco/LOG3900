using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Newtonsoft.Json.Linq;
using PolyPaint.Helpers;
using PolyPaint.Helpers.Communication;
using PolyPaint.Models.ApiModels;
using PolyPaint.Models.MessagingModels;

namespace PolyPaint.ViewModels
{
    /// <summary>
    ///     Expose the commands and properties connected with the model to the 
    ///     elements of the HistoryWindowView.
    ///     Receive changes from the model and send the Send Requests to the View
    /// </summary>
    internal class HistoryWindowViewModel : ViewModelBase , INotifyPropertyChanged
    {
        public HistoryWindowViewModel()
        {
            LoadActionHistory(1);

            RefreshCurrentPageCommand = new RelayCommand<object>(RefreshCurrentPage);
            LoadNextPageCommand = new RelayCommand<object>(LoadNextPage, CanLoadNext);
            LoadPreviousPageCommand = new RelayCommand<object>(LoadPreviousPage, CanLoadPrevious);
        }

        private void LoadPreviousPage(object obj) => LoadActionHistory(_currentPage - 1);

        private void LoadNextPage(object obj) => LoadActionHistory(_currentPage + 1);

        private void RefreshCurrentPage(object obj) => LoadActionHistory(_currentPage);

        public RelayCommand<object> RefreshCurrentPageCommand { get; set; }
        public RelayCommand<object> LoadNextPageCommand { get; set; }
        public RelayCommand<object> LoadPreviousPageCommand { get; set; }

        private bool CanLoadPrevious(object o) => CurrentPage > 1;
        private bool CanLoadNext(object o) => CurrentPage < MaxPages;


        private int _currentPage = 1;
        private int _maxPages = 1;

        private int CurrentPage
        {
            get => _currentPage;
            set
            {
                _currentPage = value;
                PropertyModified("PageIndex");
            }
        }

        private int MaxPages
        {
            get => _maxPages;
            set
            {
                _maxPages = value;
                PropertyModified("PageIndex");
            }
        }

        public string PageIndex => $"{CurrentPage} / {MaxPages}";

        private ObservableCollection<HistoryDisplayModel> _drawingActionHistory;

        public ObservableCollection<HistoryDisplayModel> HistoryChanges
        {
            get => _drawingActionHistory;
            set
            {
                _drawingActionHistory = value;
                PropertyModified();
            }
        }
        
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///     Called when a property of the ViewModel is changed.
        ///     An event is sent by the Viewmodel then
        ///     The event contains the name of the property modified. The event will be
        ///     catched by the View and the View will then update the component concerned
        /// </summary>
        protected virtual void PropertyModified([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            
        }

        private async void LoadActionHistory(int page)
        {
            HttpResponseMessage response = await RestHandler.GetDrawingActionsHistory(DrawingRoomId, page);

            DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0);

            if (response.IsSuccessStatusCode)
            {
                try
                {
                    JObject content = JObject.Parse(await response.Content.ReadAsStringAsync());
                    MaxPages = content["pages"].ToObject<int>();
                    CurrentPage = content["page"].ToObject<int>();
                    ObservableCollection<HistoryChangeModel> docs =
                        content["docs"].ToObject<ObservableCollection<HistoryChangeModel>>();

                    HistoryChanges = new ObservableCollection<HistoryDisplayModel>();
                    foreach (HistoryChangeModel item in docs)
                    {
                        HistoryDisplayModel historyItem = new HistoryDisplayModel
                        {
                            Author = item.Author.Username,
                            Timestamp = unixEpoch.AddMilliseconds(item.Timestamp).ToLocalTime()
                        };

                        FormatHistoryMessageItem(historyItem, item);

                        HistoryChanges.Add(historyItem);
                    }
                }
                catch
                {
                    // ignored
                }
            }

            RefreshHistoryBindings();
        }

        private static void RefreshHistoryBindings()
        {
            (Application.Current?.Dispatcher ?? Dispatcher.CurrentDispatcher).Invoke(CommandManager
                                                                                         .InvalidateRequerySuggested);
        }

        private void FormatHistoryMessageItem(HistoryDisplayModel historyItem, HistoryChangeModel item)
        {
            if (PixelEditor == null && StrokeEditor != null)
            {
                switch ((StrokeActionIds)item.ActionId)
                {
                    case StrokeActionIds.NewStroke:
                        historyItem.ActionDescription = "Ajout d'un nouveau trait";
                        break;
                    case StrokeActionIds.ReplaceStroke:
                        historyItem.ActionDescription = "Remplacement de trait";
                        break;
                    case StrokeActionIds.Transform:
                        historyItem.ActionDescription = "Transformation de trait";
                        break;
                    case StrokeActionIds.LockStrokes:
                        historyItem.ActionDescription = "Blocage de l'accès à un trait";
                        break;
                    case StrokeActionIds.UnlockStrokes:
                        historyItem.ActionDescription = "Déblocage de l'accès à un trait";
                        break;
                    case StrokeActionIds.Reset:
                        historyItem.ActionDescription = "Réinitialisation du dessin";
                        break;
                    default:
                        historyItem.ActionDescription = "L'action est inconnue";
                        break;
                }
            }
            else if (PixelEditor != null && StrokeEditor == null)
            {
                switch ((PixelActionIds)item.ActionId)
                {
                    case PixelActionIds.NewPixels:
                        historyItem.ActionDescription = "Écriture de pixels";
                        break;
                    case PixelActionIds.Lock:
                        historyItem.ActionDescription = "Blocage de l'accès à une zone de pixels";
                        break;
                    case PixelActionIds.Unlock:
                        historyItem.ActionDescription = "Déblocage de l'accès à une zone de pixels";
                        break;
                    default:
                        historyItem.ActionDescription = "L'action est inconnue";
                        break;
                }
            }
        }
    }
}
