﻿namespace PolyPaint.Constants
{
    /// <summary>
    ///     This class contains only the constants necessary for generating the proper JSON key/value pairs to insure proper
    ///     communication between the heavy client and the server.
    /// </summary>
    public static class JsonConstantStrings
    {
        public const string TypeChatMessageOutgoingValue = "client.chat.message";
        public const string TypeChatMessageIncomingValue = "server.chat.message";
        public const string TypeStrokeEditorActionOutgoingValue = "client.editor.stroke.action";
        public const string TypeStrokeEditorActionIncomingValue = "server.editor.stroke.action";
        public const string TypePixelEditorActionOutgoingValue = "client.editor.pixel.action";
        public const string TypePixelEditorActionIncomingValue = "server.editor.pixel.action";
        public const string TypeEditorSubscriptionOutgoingValue = "client.editor.subscription";
        public const string TypeEditorPollIncomingValue = "server.editor.poll";
    }
}
