using Microsoft.VisualStudio.TestTools.UnitTesting;
using PolyPaint.Utilitaires;

namespace PolyPaintTests.Utilitaires
{
    [TestClass]
    public class MessengerTest
    {
        private const string Uri = "ws://localhost:3000";
        private static Messenger _msSucess;
        private static Messenger _msFail;

        [ClassInitialize()]
        public static void InitializeMessenger(TestContext context)
        {
            _msSucess = new Messenger(Uri, new SocketHandlerMock(Uri));
            _msFail = new Messenger(Uri, new SocketHandlerMockFail(Uri));
        }

        [TestMethod]
        public void SendMessage1()
        {
            const string testString = "";

            string realOutputString = _msSucess.SendMessage(testString);

            Assert.AreEqual(testString, realOutputString,
                            "An empty string in input should return an empty string as output");
        }

        [TestMethod]
        public void SendMessage2()
        {
            const string testMessage = "This is a test message";

            const string expectedOutputString = "{\r\n  \"type\": \"client.chat.message\",\r\n  \"message\": \"" +
                                                testMessage + "\"\r\n}";

            string realOutputString = _msSucess.SendMessage(testMessage);

            Assert.AreEqual(expectedOutputString, realOutputString, "Should return JSON-formatted string");
        }

        [TestMethod]
        public void SendMessage3()
        {
            const string testMessage = "This is a test message containing \" quotes \"";

            const string expectedOutputString =
                "{\r\n  \"type\": \"client.chat.message\",\r\n  \"message\": \"This is a test message containing \\\" quotes \\\"\"\r\n}";

            string realOutputString = _msSucess.SendMessage(testMessage);

            Assert.AreEqual(expectedOutputString, realOutputString,
                            "Should return JSON-formatted string without breaking due to quotes");
        }

        [TestMethod]
        public void SendMessage4()
        {
            const string testMessage = "The content of the message doesn't matter";

            const string expectedOutputString = "";

            string realOutputString = _msFail.SendMessage(testMessage);

            Assert.AreEqual(expectedOutputString, realOutputString,
                            "Should return an empty string because SocketHandler failed to send");
        }

        private class SocketHandlerMock : ISocketHandler
        {
            public SocketHandlerMock(string uri)
            {
            }

            bool ISocketHandler.SendMessage(string data)
            {
                return true;
            }
        }

        private class SocketHandlerMockFail : ISocketHandler
        {
            public SocketHandlerMockFail(string uri)
            {
            }

            bool ISocketHandler.SendMessage(string data)
            {
                return false;
            }
        }
    }
}