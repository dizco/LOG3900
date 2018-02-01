﻿using System;
using PolyPaint.Modeles.MessagingModels;
using PolyPaint.Utilitaires;

namespace PolyPaint.VueModeles
{
    internal class ViewModelBase
    {
        private static Messenger _messenger;

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
        /// Connects the WebSocket to the specified URL
        /// Assigns EventHandlers to SocketHandler's EventHandlers in order to capture raised events
        /// Instanciates _messenger Singleton
        /// </summary>
        /// <param name="uri">Server URI</param>
        /// <returns>Singleton instance of Messenger</returns>
        protected static Messenger StartMessenger(string uri)
        {
            if (_messenger == null)
            {
                SocketHandler socketHandler = new SocketHandler(uri);
                socketHandler.ChatMessageReceived += ChatMessageReceived;
                socketHandler.EditorActionReceived += EditorActionReceived;
                _messenger = new Messenger(socketHandler);
            }
            return _messenger;
        }
    }
}