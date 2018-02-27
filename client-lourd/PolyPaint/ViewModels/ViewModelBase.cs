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
        public static ChatWindowView ChatWindow { get; set; }

        protected static string DrawingRoomId
        {
            get => Messenger.DrawingRoomId;
            set => Messenger.DrawingRoomId = value;
        }

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
                _messenger = new Messenger(socketHandler);
            }

            return _messenger;
        }

        public static void OnChatMessageReceived(object sender, ChatMessageModel chatMessageModel)
        {
            ChatMessageReceived?.Invoke(null, chatMessageModel);
        }

        public static void OnEditorActionReceived(object sender, EditorActionModel editorActionModel)
        {
            EditorActionReceived?.Invoke(null, editorActionModel);
        }
    }
}