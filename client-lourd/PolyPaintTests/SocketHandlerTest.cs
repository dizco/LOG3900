using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PolyPaint.Utilitaires;

namespace PolyPaintTests
{
    /// <summary>
    /// Summary description for SocketHandlerTest
    /// </summary>
    [TestClass]
    public class SocketHandlerTest
    {
        
        [TestInitialize]
        public void SocketHandlerInit()
        {
            sh = new SocketHandler("ws://localhost:3000/chat");
        }

        private SocketHandler sh;


        [TestMethod]
        public void TestSend()
        {
            while (!sh.SendMessage("Test"));
        }
    }
}
