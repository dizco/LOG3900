using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using PolyPaint.Annotations;
using PolyPaint.Helpers.Communication;
using PolyPaint.Models.ApiModels;
using PolyPaint.Views.Gallery;

namespace PolyPaint.ViewModels.Gallery
{
    internal class GalleryViewModel : ViewModelBase, INotifyPropertyChanged, IDisposable
    {
        private readonly ISet<string> _currentUserDrawingsId;
        private readonly ISet<string> _publicDrawingsId;
        private ObservableCollection<GalleryItemView> _currentUserDrawings;
        private ObservableCollection<GalleryItemView> _publicDrawings;

        public GalleryViewModel()
        {
            CurrentUserDrawings = new ObservableCollection<GalleryItemView>();
            PublicDrawings = new ObservableCollection<GalleryItemView>();

            _currentUserDrawingsId = new HashSet<string>();
            _publicDrawingsId = new HashSet<string>();

            InitialLoadUserDrawings();
            InitialLoadPublicDrawings();
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

        private async void InitialLoadUserDrawings()
        {
            List<Tuple<string, string, bool>> userDrawings = await FetchAllOwnerDrawings();

            CurrentUserDrawings = new ObservableCollection<GalleryItemView>();

            foreach (Tuple<string, string, bool> drawing in userDrawings)
            {
                _currentUserDrawingsId.Add(drawing.Item2);
                CurrentUserDrawings.Add(new GalleryItemView(drawing.Item1, drawing.Item2, true, drawing.Item3));
            }
        }

        private async void InitialLoadPublicDrawings()
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
                PublicDrawings.Add(new GalleryItemView(drawing.Item1, drawing.Item2, false, drawing.Item3));
            }
        }

        /// <summary>
        ///     Fetches drawings for which the owner is the current user
        /// </summary>
        /// <returns>A tuple in which Item1 is the drawing name, Item2 is the drawing id, Item3 is the protection status</returns>
        private static async Task<List<Tuple<string, string, bool>>> FetchAllOwnerDrawings()
        {
            int currentPage = 1;
            int maxPages = 0;

            List<Tuple<string, string, bool>> currentUserDrawings = new List<Tuple<string, string, bool>>();

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
                        currentUserDrawings.Add(new Tuple<string, string, bool>(drawing.Name, drawing.Id,
                                                                                drawing.Protection.Active));
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
