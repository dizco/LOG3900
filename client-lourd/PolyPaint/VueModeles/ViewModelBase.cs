using System;
using PolyPaint.Modeles.MessagingModels;
using PolyPaint.Utilitaires;

namespace PolyPaint.VueModeles
{
    internal class ViewModelBase
    {
        private static Messenger _messenger;

        protected Messenger Messenger => _messenger;

        protected static event EventHandler<ChatMessageModel> ChatMessageReceived;
        protected static event EventHandler<EditorActionModel> EditorActionReceived;

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