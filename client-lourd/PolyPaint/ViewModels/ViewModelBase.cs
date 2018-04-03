using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;
using PolyPaint.Helpers.Communication;
using PolyPaint.Models.MessagingModels;
using PolyPaint.Views;
using PolyPaint.Views.Gallery;

namespace PolyPaint.ViewModels
{
    internal class ViewModelBase
    {
        private static Messenger _messenger;

        private static readonly object Lock = new object();

        private static string _username;

        public static string Username
        {
            get => _username;
            set
            {
                _username = value;
                LoginStatusChanged?.Invoke(null, value);
            }
        }

        public static string UserId { get; set; }

        public static LoginWindowView LoginWindow { get; set; }
        public static HomeMenu HomeMenu { get; set; }
        public static ChatWindowView ChatWindow { get; set; }
        public static PixelEditorView PixelEditor { get; set; }
        public static StrokeEditorView StrokeEditor { get; set; }
        public static GalleryWindowView GalleryWindow { get; set; }
        public static bool IsDrawingOwner { get; set; }
        public static bool IsPasswordProtected { get; set; }
        public static bool IsPubliclyVisible { get; set; }

        protected static string DrawingRoomId
        {
            get => Messenger.DrawingRoomId;
            set => Messenger.DrawingRoomId = value;
        }

        public static string DrawingName { get; set; }

        public static ObservableCollection<ChatMessageDisplayModel> ChatMessageCollection { get; } =
            new ObservableCollection<ChatMessageDisplayModel>();

        protected Messenger Messenger => _messenger;

        internal static event EventHandler<EditorChatDisplayOptions> ChangeEditorChatDisplayState;

        protected static void OpenEditorChat()
        {
            ChatWindow = null;
            if (_messenger?.IsConnected ?? false)
            {
                ChangeEditorChatDisplayState?.Invoke(null, EditorChatDisplayOptions.Display);
            }
        }

        protected static void CloseAllChat()
        {
            ChangeEditorChatDisplayState?.Invoke(null, EditorChatDisplayOptions.Hide);
            ChatWindow?.Close();
            ChatWindow = null;
        }

        protected static void ToggleChat(object o)
        {
            if (ChatWindow == null)
            {
                ChatWindow = new ChatWindowView();
                ChatWindow.Closing += (s, a) => OpenEditorChat();
                ChatWindow.Show();
                ChangeEditorChatDisplayState?.Invoke(null, EditorChatDisplayOptions.Hide);
            }
            else
            {
                ChatWindow.Close();
                OpenEditorChat();
            }
        }

        /// <summary>
        ///     StrokeEditorActionReceived event is available for the editor to declare it's own EventHandler in order to process
        ///     incoming StrokeEditorActions from the server. The event is raised by the SocketHandler class
        /// </summary>
        protected static event EventHandler<StrokeEditorActionModel> StrokeEditorActionReceived;

        /// <summary>
        ///     PixelEditorActionReceived event is available for the editor to declare it's own EventHandler in order to process
        ///     incoming PixelEditorActions from the server. The event is raised by the SocketHandler class
        /// </summary>
        protected static event EventHandler<PixelEditorActionModel> PixelEditorActionReceived;

        /// <summary>
        ///     EditorPollRequestReceived event is available for the editors to declare their own EventHandler in order to process
        ///     an incoming request to update the thumbnail of the current drawing
        /// </summary>
        protected static event EventHandler EditorPollRequestReceived;

        /// <summary>
        ///     Raises event once the WebSocket is connected to refresh bindings that depend on it (HomeMenu)
        /// </summary>
        protected static event EventHandler WebSocketConnectedEvent;

        /// <summary>
        ///     Raises event once the WebSocket disconnects with the disconnect reason code to allow reconnection or return to
        ///     HomeMenu
        /// </summary>
        protected static event EventHandler<int> WebSocketDisconnectedEvent;

        /// <summary>
        ///     LoginSatusChanged event is raised when user logs in or logs out to notify all inherited classes of the change
        /// </summary>
        protected static event EventHandler<string> LoginStatusChanged;

        /// <summary>
        ///     Connects the WebSocket to the specified URL
        ///     Assigns EventHandlers to SocketHandler's EventHandlers in order to capture raised events
        ///     Instanciates _messenger Singleton
        /// </summary>
        /// <param name="uri">Server URI</param>
        /// <returns>Singleton instance of Messenger</returns>
        protected static Messenger StartMessenger(string uri, List<KeyValuePair<string, string>> cookies)
        {
            if (_messenger == null)
            {
                SocketHandler socketHandler = new SocketHandler(uri, cookies);
                socketHandler.ChatMessageReceived += OnChatMessageReceived;
                socketHandler.StrokeEditorActionReceived += OnStrokeEditorActionReceived;
                socketHandler.PixelEditorActionReceived += OnPixelEditorActionReceived;
                socketHandler.WebSocketConnectedEvent += OnWebSocketConnected;
                socketHandler.WebSocketDisconnectedEvent += OnWebSocketDisconnected;
                socketHandler.EditorPollRequestReceived += OnEditorPollRequestReceived;
                _messenger = new Messenger(socketHandler);

                BindingOperations.EnableCollectionSynchronization(ChatMessageCollection, Lock);
            }

            return _messenger;
        }

        private static void OnEditorPollRequestReceived(object sender, EventArgs eventArgs)
        {
            EditorPollRequestReceived?.Invoke(sender, eventArgs);
        }

        public static void OnChatMessageReceived(object sender, ChatMessageModel chatMessageModel)
        {
            AppendMessageToChat(chatMessageModel);
        }

        private static void AppendMessageToChat(ChatMessageModel incomingMessage = null)
        {
            string message, author;
            DateTime messageTime;

            if (incomingMessage != null)
            {
                DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                message = incomingMessage.Message;
                messageTime = unixEpoch.AddMilliseconds(incomingMessage.Timestamp).ToLocalTime();
                author = incomingMessage.Author?.Username ?? "PodMuncher";
            }
            else
            {
                //Invalid call to method, exit before processing any further
                return;
            }

            lock (Lock)
            {
                ChatMessageCollection.Add(new ChatMessageDisplayModel
                {
                    MessageText = message,
                    Timestamp = messageTime,
                    AuthorName = author
                });
            }
        }

        public static void OnStrokeEditorActionReceived(object sender, StrokeEditorActionModel strokeEditorActionModel)
        {
            StrokeEditorActionReceived?.Invoke(sender, strokeEditorActionModel);
        }

        private static void OnPixelEditorActionReceived(object sender, PixelEditorActionModel pixelEditorActionmodel)
        {
            PixelEditorActionReceived?.Invoke(sender, pixelEditorActionmodel);
        }

        public static void OnWebSocketConnected(object sender, EventArgs e)
        {
            (Application.Current?.Dispatcher ?? Dispatcher.CurrentDispatcher).Invoke(OpenEditorChat);
            WebSocketConnectedEvent?.Invoke(sender, e);
        }

        private static void OnWebSocketDisconnected(object sender, int e)
        {
            (Application.Current?.Dispatcher ?? Dispatcher.CurrentDispatcher).Invoke(CloseAllChat);
            WebSocketDisconnectedEvent?.Invoke(sender, e);
        }

        protected void OnEditorClosedHandler(object sender, EventArgs e)
        {
            if (sender is StrokeEditorView strokeEditorView)
            {
                (strokeEditorView.DataContext as StrokeEditorViewModel)?.UnsubscribeDrawingRoom();
                (strokeEditorView.DataContext as StrokeEditorViewModel)?.Dispose();
            }
            else if (sender is PixelEditorView pixelEditorView)
            {
                (pixelEditorView.DataContext as PixelEditorViewModel)?.UnsubscribeDrawingRoom();
                (pixelEditorView.DataContext as PixelEditorViewModel)?.Dispose();
            }

            if (HomeMenu == null)
            {
                HomeMenu = new HomeMenu();
                HomeMenu.Show();
                HomeMenu.Closing += (s, a) => HomeMenu = null;
            }

            StrokeEditor = null;
            PixelEditor = null;
            DrawingRoomId = null;
        }

        internal enum EditorChatDisplayOptions
        {
            Display,
            Hide
        }
    }
}
