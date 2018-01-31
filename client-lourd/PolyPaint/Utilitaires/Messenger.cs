using System;
using System.Windows.Ink;
using Newtonsoft.Json;
using PolyPaint.Constants;
using PolyPaint.Modeles.MessagingModels;

namespace PolyPaint.Utilitaires
{
    public class Messenger
    {
        private readonly ISocketHandler _socketHandler;

        public Messenger(ISocketHandler socketHandler)
        {
            _socketHandler = socketHandler;
        }

        public string SendChatMessage(string outgoingMessage)
        {
            if (outgoingMessage != string.Empty)
            {
                ChatMessageModel chatMessage = new ChatMessageModel
                {
                    type = JsonConstantStrings.TypeChatMessageOutgoingValue,
                    message = outgoingMessage,
                    room = new RoomModel
                    {
                        id = "chat"
                    }
                };

                string messageSerialized = JsonConvert.SerializeObject(chatMessage);

                bool isSent = _socketHandler.SendMessage(messageSerialized);

                if (isSent)
                    return messageSerialized;
            }
            return string.Empty;
        }

        private string SendEditorAction()
        {
            throw new NotImplementedException();
        }

        public string SendEditorStrokeAddedAction(object stroke)
        {
            if (stroke is Stroke)
            {
                // TODO: Convert stroke to a format server can understand
            }
            return string.Empty;
        }
    }
}