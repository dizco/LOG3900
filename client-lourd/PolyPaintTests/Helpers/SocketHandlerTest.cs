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
        private const string HttpServerUri = "http://localhost:3000";
        private SocketHandler _socketHandler;

        [TestInitialize]
        public async Task SocketHandlerInit()
        {
            
            // TODO: uncomment following code with server running to test
            //RestHandler.ServerUri = HttpServerUri;

            //CookieContainer cookies = new CookieContainer();

            //HttpResponseMessage response = await RestHandler.LoginInfo("me@me.ca", "hahahaha");

            //List<KeyValuePair<string, string>> cookiesList = cookies
            //    .GetCookies(new Uri(HttpServerUri)).Cast<Cookie>()
            //    .Select(cookie => new KeyValuePair<string, string>(cookie.Name, cookie.Value)).ToList();

            //_socketHandler = new SocketHandler("ws://localhost:3000/", cookiesList);
        }


        [TestMethod]
        public void TestSend()
        {
            Assert.IsTrue(true);
            // TODO: uncomment following code with server running to test w/ breakpoints in SocketHandler.cs
            //while (true);
        }
    }
}