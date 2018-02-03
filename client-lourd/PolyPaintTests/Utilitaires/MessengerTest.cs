using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PolyPaint.Utilitaires;

namespace PolyPaintTests.Utilitaires
{
    [TestClass]
    public class MessengerTest
    {
        private const string DummyServerUri = "ws://localhost:3000";
        private static Messenger _messenger;
        private static Messenger _messengerFail;

        [ClassInitialize]
        public static void InitializeMessenger(TestContext context)
        {
            _messenger = new Messenger(new SocketHandlerMock(DummyServerUri));
            _messengerFail = new Messenger(new SocketHandlerMockFail(DummyServerUri));
        }

        [TestMethod]
        public void TestSendChatMessageEmpty()
        {
            const string testString = "";

            string realOutputString = _messenger.SendChatMessage(testString);

            Assert.AreEqual(testString, realOutputString,
                            "An empty string in input should return an empty string as output");
        }

        [TestMethod]
        public void TestSendChatMessageSuccess()
        {
            const string testMessage = "This is a test message";

            const string expectedOutputString = "{\"message\":\"" + testMessage +
                                                "\",\"author\":null,\"room\":{\"id\":\"chat\",\"name\":null},\"timestamp\":0,\"type\":\"client.chat.message\"}";

            string realOutputString = _messenger.SendChatMessage(testMessage);


            Assert.AreEqual(expectedOutputString, realOutputString, "Should return JSON-formatted string");
        }

        [TestMethod]
        public void TestSendChatMessageWithQuotes()
        {
            const string testMessage = "This is a test message containing \" quotes \"";

            const string expectedOutputString =
                "{\"message\":\"This is a test message containing \\\" quotes \\\"\",\"author\":null,\"room\":{\"id\":\"chat\",\"name\":null},\"timestamp\":0,\"type\":\"client.chat.message\"}";

            string realOutputString = _messenger.SendChatMessage(testMessage);

            Assert.AreEqual(expectedOutputString, realOutputString,
                            "Should return JSON-formatted string without breaking due to quotes");
        }

        [TestMethod]
        public void TestSendChatMessageWithSocketFail()
        {
            const string testMessage = "The content of the message doesn't matter";

            const string expectedOutputString = "";

            string realOutputString = _messengerFail.SendChatMessage(testMessage);

            Assert.AreEqual(expectedOutputString, realOutputString,
                            "Should return an empty string because SocketHandler failed to send");
        }

        [TestMethod]
        public void TestSendEditorActionNewStrokeWithInvalidStroke()
        {
            object stroke = new object();
            string expectedOutputString = string.Empty;

            string realOutputString = _messenger.SendEditorActionNewStroke(stroke as Stroke);

            Assert.AreEqual(expectedOutputString, realOutputString,
                            "Should return empty string since stroke is not of type stroke/is null");
        }

        [TestMethod]
        public void TestSendEditorActionNewStrokeSuccess()
        {
            string expectedOutputString =
                "{\"action\":{\"id\":1,\"name\":\"NewStroke\"},\"author\":null,\"drawing\":{\"id\":null},\"stroke\":{\"drawingAttributes\":{\"color\":\"#FF000000\",\"height\":2.0031496062992127,\"width\":2.0031496062992127,\"stylusTip\":\"Ellipse\"},\"dots\":[{\"x\":1.0,\"y\":1.0},{\"x\":2.0,\"y\":2.0},{\"x\":3.0,\"y\":3.0},{\"x\":4.0,\"y\":4.0},{\"x\":5.0,\"y\":5.0},{\"x\":6.0,\"y\":6.0},{\"x\":7.0,\"y\":7.0},{\"x\":8.0,\"y\":8.0},{\"x\":9.0,\"y\":9.0},{\"x\":10.0,\"y\":10.0}]},\"type\":\"client.editor.action\"}";

            //Generate stylus points
            StylusPointCollection points = new StylusPointCollection();
            for (int i = 0; i < 10; i++)
                points.Add(new StylusPoint(i + 1, i + 1));

            //Generate drawing attributes
            DrawingAttributes attributes = new DrawingAttributes {Color = Colors.Black};

            //Create stroke
            Stroke stroke = new Stroke(points, attributes);

            string realOutputString = _messenger.SendEditorActionNewStroke(stroke);

            Assert.AreEqual(expectedOutputString, realOutputString,
                            "Should return stringified JSON of an EditorActionModel");
        }

        [TestMethod]
        public void TestSendEditorActionNewStrokeWithSocketFail()
        {
            string expectedOutputString = string.Empty;

            //Generate stylus points
            StylusPointCollection points = new StylusPointCollection();
            for (int i = 0; i < 10; i++)
                points.Add(new StylusPoint(i + 1, i + 1));

            //Generate drawing attributes
            DrawingAttributes attributes = new DrawingAttributes {Color = Colors.Black};

            //Create stroke
            Stroke stroke = new Stroke(points, attributes);

            string realOutputString = _messengerFail.SendEditorActionNewStroke(stroke);

            Assert.AreEqual(expectedOutputString, realOutputString,
                            "Should return an empty string because SocketHandler failed to send message");
        }

        private class SocketHandlerMock : ISocketHandler
        {
            public SocketHandlerMock(string uri)
            {
            }

            public bool IsConnected { get; } = true;

            public void DisconnectSocket()
            {
            }

            public void ConnectSocket()
            {
            }

            public bool SendMessage(string data)
            {
                return true;
            }
        }

        private class SocketHandlerMockFail : ISocketHandler
        {
            public SocketHandlerMockFail(string uri)
            {
            }

            public bool IsConnected { get; } = true;

            public void DisconnectSocket()
            {
            }

            public void ConnectSocket()
            {
            }


            public bool SendMessage(string data)
            {
                return false;
            }
        }
    }
}