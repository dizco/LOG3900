using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using PolyPaint.Helpers;
using PolyPaint.Models.PixelModels;
using PolyPaint.Views;
using EditorPixel = PolyPaint.Models.PixelModels.EditorPixel;

namespace PolyPaint.ViewModels
{
    /// <summary>
    ///     Sert d'intermédiaire entre la vue et le modèle.
    ///     Expose des commandes et propriétés connectées au modèle aux des éléments de la vue peuvent se lier.
    ///     Reçoit des avis de changement du modèle et envoie des avis de changements à la vue.
    /// </summary>
    internal class EditorPixelViewModel : ViewModelBase, INotifyPropertyChanged
    {
        private readonly EditorPixel _editorPixelModel = new EditorPixel();

        /// <summary>
        ///     Constructeur de VueModele
        ///     On récupère certaines données initiales du modèle et on construit les commandes
        ///     sur lesquelles la vue se connectera.
        /// </summary>
        public EditorPixelViewModel()
        {
            // On écoute pour des changements sur le modèle. Lorsqu'il y en a, DrawingPixelModelPropertyModified est appelée.
            _editorPixelModel.DrawingName = DrawingName;

            InitializeBitmap();
            // Pour les commandes suivantes, il est toujours possible des les activer.
            // Donc, aucune vérification de type Peut"Action" à faire.
            ChooseTool = new RelayCommand<string>(_editorPixelModel.SelectTool);

            ExportImageCommand = new RelayCommand<object>(_editorPixelModel.ExportImagePrompt);

            //Managing different View
            OpenChatWindowCommand = new RelayCommand<object>(OpenChatWindow, CanOpenChat);

            LoginStatusChanged += ProcessLoginStatusChange;
        }

        public WriteableBitmap WriteableBitmap
        {
            get => _editorPixelModel.WriteableBitmap;
            set => _editorPixelModel.WriteableBitmap = value;
        }

        public string ToolSelected
        {
            get => _editorPixelModel.SelectedTool;
            set => PropertyModified();
        }

        public string ColorSelected
        {
            get => _editorPixelModel.SelectedColor;
            set => _editorPixelModel.SelectedColor = value;
        }

        public int StrokeSizeSelected
        {
            get => _editorPixelModel.PixelSize;
            set => _editorPixelModel.PixelSize = value;
        }

        public Visibility ChatVisibility
        {
            get
            {
                if (Messenger?.IsConnected ?? false)
                {
                    return Visibility.Visible;
                }

                return Visibility.Hidden;
            }
        }

        //Commands for choosing the tools
        public RelayCommand<string> ChooseTool { get; set; }

        public RelayCommand<object> OpenFileCommand { get; set; }
        public RelayCommand<object> SaveFileCommand { get; set; }
        public RelayCommand<object> AutosaveFileCommand { get; set; }
        public RelayCommand<string> LoadAutosaved { get; set; }

        public RelayCommand<object> ExportImageCommand { get; set; }

        //Command for managing the views
        public RelayCommand<object> OpenChatWindowCommand { get; set; }
        public RelayCommand<object> ShowChatWindowCommand { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public void InitiateBitmap()
        {
            _editorPixelModel.InitializeBitmap();
        }

        public void InitializeBitmap()
        {
            _editorPixelModel.InitializeBitmap();
        }

        public void PixelDraw(Point oldPoint, Point newPoint)
        {
            _editorPixelModel.PixelDraw(oldPoint, newPoint);
        }

        public void PixelCursors(Border displayArea)
        {
            _editorPixelModel.PixelCursor(displayArea);
        }

        private void ProcessLoginStatusChange(object sender, string username)
        {
            _editorPixelModel.CurrentUsername = username;
        }

        /// <summary>
        ///     Appelee lorsqu'une propriété de VueModele est modifiée.
        ///     Un évènement indiquant qu'une propriété a été modifiée est alors émis à partir de VueModèle.
        ///     L'évènement qui contient le nom de la propriété modifiée sera attrapé par la vue qui pourra
        ///     alors mettre à jour les composants concernés.
        /// </summary>
        /// <param name="propertyName">Nom de la propriété modifiée.</param>
        protected virtual void PropertyModified([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        ///     Traite les évènements de modifications de propriétés qui ont été lancés à partir
        ///     du modèle.
        /// </summary>
        /// <param name="sender">L'émetteur de l'évènement (le modèle)</param>
        /// <param name="e">
        ///     Les paramètres de l'évènement. PropertyName est celui qui nous intéresse.
        ///     Il indique quelle propriété a été modifiée dans le modèle.
        /// </param>
        private bool CanOpenChat(object obj)
        {
            return Messenger?.IsConnected ?? false;
        }

        //Show login window
        public void OpenChatWindow(object o)
        {
            if (ChatWindow == null)
            {
                ChatWindow = new ChatWindowView();
                ChatWindow.Show();
                ChatWindow.Closed += (sender, args) => ChatWindow = null;
            }
            else
            {
                ChatWindow.Activate();
            }
        }
    }
}
