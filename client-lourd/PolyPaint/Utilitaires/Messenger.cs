using System;
using Newtonsoft.Json.Linq;

namespace PolyPaint.Utilitaires
{
    public class Messenger
    {
        private readonly ISocketHandler sh;

        public Messenger(string uri, ISocketHandler ish)
        {
            sh = ish;
        }

        public string SendMessage(string message)
        {
            if (message != string.Empty)
            {
                JObject messageJson = new JObject
                {
                    {JsonConstantStrings.TypeKey, JsonConstantStrings.TypeMessageValue},
                    {JsonConstantStrings.MessageKey, message}
                };

                string messageStringified = messageJson.ToString();

                bool status = sh.SendMessage(messageStringified);

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