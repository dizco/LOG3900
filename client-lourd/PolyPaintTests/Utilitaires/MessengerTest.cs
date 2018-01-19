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
            _messenger = new Messenger(DummyServerUri, new SocketHandlerMock(DummyServerUri));
            _messengerFail = new Messenger(DummyServerUri, new SocketHandlerMockFail(DummyServerUri));
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

            const string expectedOutputString = "{\r\n  \"type\": \"client.chat.message\",\r\n  \"message\": \"" +
                                                testMessage + "\"\r\n}";

            string realOutputString = _messenger.SendChatMessage(testMessage);

            Assert.AreEqual(expectedOutputString, realOutputString, "Should return JSON-formatted string");
        }

        [TestMethod]
        public void TestSendChatMessageWithQuotes()
        {
            const string testMessage = "This is a test message containing \" quotes \"";

            const string expectedOutputString =
                "{\r\n  \"type\": \"client.chat.message\",\r\n  \"message\": \"This is a test message containing \\\" quotes \\\"\"\r\n}";

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

        private class SocketHandlerMock : ISocketHandler
        {
            public SocketHandlerMock(string uri)
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

            public bool SendMessage(string data)
            {
                return false;
            }
        }
    }
}