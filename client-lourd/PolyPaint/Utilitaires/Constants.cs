namespace PolyPaint.Utilitaires
{
    /// <summary>
    ///     This class contains only the constants necessary for generating the proper JSON key/value pairs to insure proper
    ///     communication between the heavy client and the server.
    /// </summary>
    public static class JsonConstantStrings
    {
        public static string ActionKey => "action";
        public static string IdKey => "id";
        public static string MessageKey => "message";
        public static string TypeKey => "type";
        public static string TypeMessageValue => "client.chat.message";
        public static string TypeEditorActionValue => "client.editor.action";
    }

    // TODO: Add static class for other parts of the app needing constant strings
}