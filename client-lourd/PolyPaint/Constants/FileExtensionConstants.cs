﻿using System.IO;

namespace PolyPaint.Constants
{
    public static class FileExtensionConstants
    {
        public const string Filter = "PolyPaintPro File (*.tide)|*.tide";
        public const string DefaultExt = "tide";
        public const string ExportImageFilter =
            "Bitmap files (*.bmp)|*.bmp|JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|PNG (*.png)|*.png|" +
            "All files (*.*)|*.*";
        public const string ExportImgDefaultExt = "png";
        public static readonly string AutosaveDirPath = Path.GetTempPath() + "PolyPaintPro\\";
        public static readonly string AutosaveFilePath = AutosaveDirPath + "{0}_autosave.tide";
    }
}