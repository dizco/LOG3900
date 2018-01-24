using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PolyPaint.Utilitaires;

namespace PolyPaintTests.Utilitaires
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
            // To verify that socket connection is still working, uncomment and run with server
            sh = new SocketHandler("ws://localhost:3000/");
            Console.WriteLine("Socket opened");
        }

        private SocketHandler sh;


        [TestMethod]
        public void TestSend()
        {
            Assert.IsTrue(true);
            // To verify that socket connection is still working, uncomment and run with server
            while (true);
        }
    }
}
