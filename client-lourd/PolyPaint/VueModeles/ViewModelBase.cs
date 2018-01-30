using System;
using Newtonsoft.Json.Linq;
using PolyPaint.Modeles.MessagingModels;
using PolyPaint.Utilitaires;

namespace PolyPaint.VueModeles
{
    internal class ViewModelBase
    {
        private static Messenger _mesenger;

        protected Messenger Messenger => _mesenger;

        protected static event EventHandler<ChatMessageModel> ChatMessageReceived;
        protected static event EventHandler<EditorActionModel> EditorActionReceived;

        protected static Messenger StartMessenger(string uri)
        {
            if (_mesenger == null)
            {
                SocketHandler socketHandler = new SocketHandler(uri);
                socketHandler.ChatMessageReceived += ChatMessageReceived;
                socketHandler.EditorActionReceived += EditorActionReceived;
                _mesenger = new Messenger(socketHandler);
            }
            return _mesenger;
        }
    }
}