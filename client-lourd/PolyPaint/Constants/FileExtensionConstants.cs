using System.IO;

namespace PolyPaint.Constants
{
    public static class FileExtensionConstants
    {
        public const string Filter = "PolyPaintPro File (*.tide)|*.tide";
        public const string DefaultExt = "tide";
        public static readonly string AutosavePath = Path.GetTempPath() + "PolyPaintPro\\";
    }
}