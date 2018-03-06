using System;
using System.Collections.Generic;
using PolyPaint.Helpers.Communication;
using PolyPaint.Models.MessagingModels;
using PolyPaint.Views;

namespace PolyPaint.ViewModels
{
    internal class ViewModelBase
    {
        private static Messenger _messenger;

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

        public static LoginWindowView LoginWindow { get; set; }
        public static HomeMenu HomeMenu { get; set; }
        public static ChatWindowView ChatWindow { get; set; }
        public static DrawingWindow EditorWindow { get; set; }

        protected static string DrawingRoomId
        {
            get => Messenger.DrawingRoomId;
            set => Messenger.DrawingRoomId = value;
        }

        protected static string DrawingName { get; set; }

        protected Messenger Messenger => _messenger;

        /// <summary>
        ///     ChatMessageReceived event is available for classes that inherit from ViewModelBase to declare their own
        ///     EventHandler for incoming chat message data once an event is raised in the SocketHandler class
        /// </summary>
        protected static event EventHandler<ChatMessageModel> ChatMessageReceived;

        /// <summary>
        ///     EditorActionReceived event is available for the editor to declare it's own EventHandler in order to process
        ///     incoming EditorActions from the server. The event is raised by the SocketHandler class
        /// </summary>
        protected static event EventHandler<EditorActionModel> EditorActionReceived;

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
                socketHandler.EditorActionReceived += OnEditorActionReceived;
                socketHandler.WebSocketConnectedEvent += OnWebSocketConnected;
                socketHandler.WebSocketDisconnectedEvent += OnWebSocketDisconnected;
                _messenger = new Messenger(socketHandler);
            }

            return _messenger;
        }

        public static void OnChatMessageReceived(object sender, ChatMessageModel chatMessageModel)
        {
            ChatMessageReceived?.Invoke(sender, chatMessageModel);
        }

        public static void OnEditorActionReceived(object sender, EditorActionModel editorActionModel)
        {
            EditorActionReceived?.Invoke(sender, editorActionModel);
        }

        public static void OnWebSocketConnected(object sender, EventArgs e)
        {
            WebSocketConnectedEvent?.Invoke(sender, e);
        }

        private static void OnWebSocketDisconnected(object sender, int e)
        {
            WebSocketDisconnectedEvent?.Invoke(sender, e);
        }
    }
}
