﻿using System;
using Newtonsoft.Json.Linq;
using PolyPaint.Constants;

namespace PolyPaint.Utilitaires
{
    public class Messenger
    {
        private readonly ISocketHandler _socketHandler;

        public Messenger(string uri, ISocketHandler socketHandler)
        {
            _socketHandler = socketHandler;
        }

        public string SendChatMessage(string message)
        {
            if (message != string.Empty)
            {
                JObject messageJson = new JObject
                {
                    {JsonConstantStrings.TypeKey, JsonConstantStrings.TypeChatMessageValue},
                    {JsonConstantStrings.MessageKey, message}
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