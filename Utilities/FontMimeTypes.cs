using System.IO;

namespace Etch.OrchardCore.Fonts.Utilities
{
    public static class FontMimeTypes
    {
        public static string GetMimetype(string path)
        {
            switch (Path.GetExtension(path))
            {
                case ".woff":
                    return "font/woff";
                case ".woff2":
                    return "font/woff2";
                default:
                    return string.Empty;
            }
        }
    }
}
