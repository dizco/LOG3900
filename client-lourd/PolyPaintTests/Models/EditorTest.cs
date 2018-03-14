using System;
using System.Windows.Ink;
using System.Windows.Input;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PolyPaint.CustomComponents;
using PolyPaint.Models;

namespace PolyPaintTests.Models
{
    [TestClass]
    public class EditorTest
    {
        private static EditorStroke _editor;

        [ClassInitialize]
        public static void InitializeMessenger(TestContext context)
        {
            _editor = new EditorStroke();
        }

        [TestMethod]
        public void TestAssignUuidToStroke()
        {
            Stroke stroke = new Stroke(new StylusPointCollection(new[] {new StylusPoint(1, 2)}));

            Stroke customStroke = _editor.AssignUuidToStroke(stroke);

            Assert.AreEqual(typeof(CustomStroke), typeof(CustomStroke),
                            "AssignUuidToStroke should return a CustomStroke");
            Assert.IsTrue(((CustomStroke) customStroke).Uuid != null);
            Assert.AreNotSame(Guid.Empty.ToString(), ((CustomStroke) customStroke).Uuid,
                              "Assigned UUID should not be an empty UUID");
        }
    }
}
