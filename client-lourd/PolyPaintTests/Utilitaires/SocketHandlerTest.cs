using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PolyPaint.Utilitaires;

namespace PolyPaintTests.Utilitaires
{
    /// <summary>
    ///     Summary description for SocketHandlerTest
    /// </summary>
    [TestClass]
    public class SocketHandlerTest
    {
        private SocketHandler _socketHandler;

        [TestInitialize]
        public void SocketHandlerInit()
        {
            _socketHandler = new SocketHandler("ws://localhost:3000/");
        }


        [TestMethod]
        public void TestSend()
        {
            Assert.IsTrue(true);
            //To verify something with socket, uncomment and set breakpoints in SocketHandler.cs
            //while (true);
        }
    }
}