using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PolyPaint.Helpers.Communication;

namespace PolyPaintTests.Helpers
{
    /// <summary>
    ///     This class is for manual testing of the websockets
    /// </summary>
    [TestClass]
    public class SocketHandlerTest
    {
        private const string HttpServerUri = "http://localhost:5025";
        private SocketHandler _socketHandler;

        [TestInitialize]
        public async Task SocketHandlerInit()
        {
            // TODO: uncomment following code with server running to test
            /*RestHandler.ServerUri = HttpServerUri;

            CookieContainer cookies = new CookieContainer();
            RestHandler.Handler.CookieContainer = cookies;

            HttpResponseMessage response = await RestHandler.LoginUser("me@me.ca", "hahahaha");
            
            List<KeyValuePair<string, string>> cookiesList = cookies
                .GetCookies(new Uri(HttpServerUri)).Cast<Cookie>()
                .Select(cookie => new KeyValuePair<string, string>(cookie.Name, cookie.Value)).ToList();

            _socketHandler = new SocketHandler("ws://localhost:5025/", cookiesList);*/
        }

        [TestMethod]
        public void TestSend()
        {
            Assert.IsTrue(true);

            // TODO: uncomment following code with server running to test w/ breakpoints in SocketHandler.cs
            /*AutoResetEvent waitSocket = new AutoResetEvent(false);
            _socketHandler.WebSocketConnectedEvent += (s,e) => waitSocket.Set();
            waitSocket.WaitOne(TimeSpan.FromMinutes(1));

            _socketHandler
                .SendMessage("{\"type\": \"client.editor.action\",\"action\": {\"id\": 1,\"name\": \"init\",\"drawing\":{\"id\": \"5ab25d6e7b46256db0a83ea8\"},\"delta\": {\"add\": [{\"strokeUuid\": \"0\",\"strokeAttributes\": {\"color\": \"#FF000000\",\"height\": 11,\"width\": 11,\"stylusTip\": \"Ellipse\"},\"dots\": [{\"x\": 418,\"y\": 119.53999999999999}]}],\"remove\": []}}");
            
            for (int i = 0; i < 10; i++)
            {
                _socketHandler
                    .SendMessage("{\"type\": \"client.editor.action\",\"action\": {\"id\": 2,\"name\": "+i+"},\"drawing\":{\"id\": \"5ab25d6e7b46256db0a83ea8\"},\"delta\": {\"add\": [{\"strokeUuid\": "+(i+1)+",\"strokeAttributes\": {\"color\": \"#FF000000\",\"height\": 11,\"width\": 11,\"stylusTip\": \"Ellipse\"},\"dots\": [{\"x\": 418,\"y\": 119.53999999999999}]}],\"remove\": [\""+i+"\"]}}");
            }*/
        }
    }
}
