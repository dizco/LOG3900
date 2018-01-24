namespace PolyPaint.Constants
{
    /// <summary>
    ///     This class contains only the constants necessary for generating the proper JSON key/value pairs to insure proper
    ///     communication between the heavy client and the server.
    /// </summary>
    public static class JsonConstantStrings
    {
        public const string ActionKey = "action";
        public const string IdKey = "id";
        public const string MessageKey = "message";
        public const string TypeKey = "type";
        public const string TypeChatMessageOutgoingValue = "client.chat.message";
        public const string TypeChatMessageIncomingValue = "server.chat.message";
        public const string TypeEditorActionValue = "client.editor.action";
    }

    // TODO: Add static class for other parts of the app needing constant strings
}