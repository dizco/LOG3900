using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Newtonsoft.Json.Linq;
using PolyPaint.Annotations;
using PolyPaint.Helpers.Communication;
using PolyPaint.Models.ApiModels;
using PolyPaint.Views.Gallery;

namespace PolyPaint.ViewModels.Gallery
{
    internal class GalleryViewModel : ViewModelBase, INotifyPropertyChanged, IDisposable
    {
        private const int RefreshTimeoutSeconds = 10;
        private readonly CancellationToken _cancellationToken;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly ISet<string> _currentUserDrawingsId;
        private ObservableCollection<GalleryItemView> _currentUserDrawings;
        private ObservableCollection<GalleryItemView> _publicDrawings;
        private ISet<string> _publicDrawingsId;
        private Task _refreshDrawingListTask;

        public GalleryViewModel()
        {
            CurrentUserDrawings = new ObservableCollection<GalleryItemView>();
            PublicDrawings = new ObservableCollection<GalleryItemView>();

            _currentUserDrawingsId = new HashSet<string>();
            _publicDrawingsId = new HashSet<string>();

            _cancellationToken = new CancellationToken();

            LoadDrawings();
        }

        public ObservableCollection<GalleryItemView> CurrentUserDrawings
        {
            get => _currentUserDrawings;
            set
            {
                _currentUserDrawings = value;
                PropertyModified();
            }
        }

        public ObservableCollection<GalleryItemView> PublicDrawings
        {
            get => _publicDrawings;
            set
            {
                _publicDrawings = value;
                PropertyModified();
            }
        }

        public void Dispose()
        {
            foreach (GalleryItemView drawing in CurrentUserDrawings)
            {
                (drawing.DataContext as GalleryItemViewModel)?.Dispose();
            }

            foreach (GalleryItemView drawing in PublicDrawings)
            {
                (drawing.DataContext as GalleryItemViewModel)?.Dispose();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///     Loads user's drawing before public drawings to avoid duplicates
        /// </summary>
        private async void LoadDrawings()
        {
            await InitialLoadUserDrawings();
            await InitialLoadPublicDrawings();
            _refreshDrawingListTask = new Task(RefreshDrawings, _cancellationToken);
            _refreshDrawingListTask.Start();
        }

        private async void RefreshDrawings()
        {
            Thread.Sleep(TimeSpan.FromSeconds(RefreshTimeoutSeconds));
            if (_cancellationToken.IsCancellationRequested)
            {
                return;

            }
            await RefreshUserDrawings();
            await RefreshPublicDrawings();
            OnRefreshDrawingListTaskCompleted();
        }

        protected virtual void OnRefreshDrawingListTaskCompleted()
        {
            _refreshDrawingListTask = new Task(RefreshDrawings, _cancellationToken);
            if (!_cancellationToken.IsCancellationRequested)
            {
                _refreshDrawingListTask.Start();
            }
        }

        public event EventHandler ClosingRequest;

        private void OnClosingRequest()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            ClosingRequest?.Invoke(this, null);
        }

        private async Task InitialLoadUserDrawings()
        {
            List<Tuple<string, string, bool, bool>> userDrawings = await FetchAllOwnerDrawings();

            CurrentUserDrawings = new ObservableCollection<GalleryItemView>();

            foreach (Tuple<string, string, bool, bool> drawing in userDrawings)
            {
                _currentUserDrawingsId.Add(drawing.Item2);
                GalleryItemView item =
                    new GalleryItemView(drawing.Item1, drawing.Item2, true, drawing.Item3, drawing.Item4);

                item.ClosingRequest += (sender, args) => OnClosingRequest();

                CurrentUserDrawings.Add(item);
            }
        }

        /// <summary>
        ///     Fetches the updated list of drawings owned by current user.
        /// </summary>
        /// <returns>Task completed</returns>
        private async Task RefreshUserDrawings()
        {
            List<Tuple<string, string, bool, bool>> userDrawings = await FetchAllOwnerDrawings();

            foreach (Tuple<string, string, bool, bool> drawing in userDrawings)
            {
                if (_currentUserDrawingsId.Contains(drawing.Item2))
                {
                    continue;
                }

                _currentUserDrawingsId.Add(drawing.Item2);

                (Application.Current?.Dispatcher ?? Dispatcher.CurrentDispatcher).Invoke(() =>
                {
                    GalleryItemView item =
                        new GalleryItemView(drawing.Item1, drawing.Item2, true, drawing.Item3, drawing.Item4);

                    item.ClosingRequest += (sender, args) => OnClosingRequest();

                    CurrentUserDrawings.Add(item);
                });
            }
        }

        /// <summary>
        ///     Updates the list of public drawings.
        ///     To do so, the function creates a Set of the currently accessible drawing IDs.
        ///     If a drawing was already present, it is removed from the _publicDrawingsId list.
        ///     All remaining drawings in _publicDrawingsId are now outdated and can be removed from the list.
        ///     Replaces the HashSet of _publicDrawingsId by the most up-to-date one.
        /// </summary>
        /// <returns></returns>
        private async Task RefreshPublicDrawings()
        {
            List<Tuple<string, string, bool>> publicDrawings = await FetchAllPublicDrawings();

            ISet<string> updatedDrawingsId = new HashSet<string>();

            foreach (Tuple<string, string, bool> drawing in publicDrawings)
            {
                if (_publicDrawingsId.Contains(drawing.Item2))
                {
                    updatedDrawingsId.Add(drawing.Item2);
                    _publicDrawingsId.Remove(drawing.Item2);
                }
                else if (!_currentUserDrawingsId.Contains(drawing.Item2))
                {
                    updatedDrawingsId.Add(drawing.Item2);

                    (Application.Current?.Dispatcher ?? Dispatcher.CurrentDispatcher).Invoke(() =>
                    {
                        GalleryItemView item =
                            new GalleryItemView(drawing.Item1, drawing.Item2, true, drawing.Item3, true);

                        item.ClosingRequest += (sender, args) => OnClosingRequest();

                        PublicDrawings.Add(item);
                    });
                }
            }

            foreach (string drawingId in _publicDrawingsId)
            {
                GalleryItemView itemToRemove =
                    PublicDrawings.First(item => (item.DataContext as GalleryItemViewModel)?.DrawingId ==
                                                 drawingId);

                (Application.Current?.Dispatcher ?? Dispatcher.CurrentDispatcher).Invoke(() =>
                {
                    PublicDrawings
                        .Remove(itemToRemove);
                });
            }

            _publicDrawingsId = updatedDrawingsId;
        }

        private async Task InitialLoadPublicDrawings()
        {
            List<Tuple<string, string, bool>> publicDrawings = await FetchAllPublicDrawings();

            PublicDrawings = new ObservableCollection<GalleryItemView>();
            foreach (Tuple<string, string, bool> drawing in publicDrawings)
            {
                if (_currentUserDrawingsId.Contains(drawing.Item2))
                {
                    continue;
                }

                _publicDrawingsId.Add(drawing.Item2);

                GalleryItemView item = new GalleryItemView(drawing.Item1, drawing.Item2, false, drawing.Item3, true);

                item.ClosingRequest += (sender, args) => OnClosingRequest();

                PublicDrawings.Add(item);
            }
        }

        /// <summary>
        ///     Fetches drawings for which the owner is the current user
        /// </summary>
        /// <returns>
        ///     A tuple in which Item1 is the drawing name, Item2 is the drawing id, Item3 is the protection status, Item4 is
        ///     visibility
        /// </returns>
        private static async Task<List<Tuple<string, string, bool, bool>>> FetchAllOwnerDrawings()
        {
            int currentPage = 1;
            int maxPages = 0;

            List<Tuple<string, string, bool, bool>> currentUserDrawings = new List<Tuple<string, string, bool, bool>>();

            do
            {
                HttpResponseMessage response = await RestHandler.GetOnlineDrawingsForAuthor(UserId, currentPage);
                if (!response.IsSuccessStatusCode)
                {
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
                        currentUserDrawings.Add(new Tuple<string, string, bool, bool>(drawing.Name, drawing.Id,
                                                                                      drawing.Protection.Active,
                                                                                      drawing.Visibility == "public"));
                    }
                }
                catch
                {
                    continue;
                }

                currentPage++;
            } while (currentPage < maxPages);

            return currentUserDrawings;
        }

        private static async Task<List<Tuple<string, string, bool>>> FetchAllPublicDrawings()
        {
            int currentPage = 1;
            int maxPages = 0;

            List<Tuple<string, string, bool>> publicDrawings = new List<Tuple<string, string, bool>>();

            do
            {
                HttpResponseMessage response = await RestHandler.GetPublicOnlineDrawings(currentPage);

                if (!response.IsSuccessStatusCode)
                {
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
                        publicDrawings.Add(new Tuple<string, string, bool>(drawing.Name, drawing.Id,
                                                                           drawing.Protection.Active));
                    }
                }
                catch
                {
                    continue;
                }

                currentPage++;
            } while (currentPage < maxPages);

            return publicDrawings;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void PropertyModified([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
