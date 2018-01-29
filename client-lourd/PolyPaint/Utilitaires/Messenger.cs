using System;
using Newtonsoft.Json.Linq;
using PolyPaint.Constants;

namespace PolyPaint.Utilitaires
{
    public class Messenger
    {
        private readonly ISocketHandler _socketHandler;

        public Messenger(ISocketHandler socketHandler)
        {
            _socketHandler = socketHandler;
        }

        public string SendChatMessage(string message)
        {
            if (message != string.Empty)
            {
                JObject roomJson = new JObject
                {
                    {JsonConstantStrings.IdKey, "chat"}
                };

                JObject messageJson = new JObject
                {
                    {JsonConstantStrings.TypeKey, JsonConstantStrings.TypeChatMessageOutgoingValue},
                    {JsonConstantStrings.MessageKey, message},
                    {JsonConstantStrings.RoomKey, roomJson}
                };

                string messageStringified = messageJson.ToString();

                bool status = _socketHandler.SendMessage(messageStringified);

                if (status)
                    return messageStringified;
            }
            return string.Empty;
        }

        public string SendEditorAction()
        {
            throw new NotImplementedException();
        }
    }
}