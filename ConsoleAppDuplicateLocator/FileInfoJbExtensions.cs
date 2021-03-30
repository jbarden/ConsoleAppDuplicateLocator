using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppDuplicateLocator
{
    internal static class FileInfoJbExtensions
    {
        public static bool IsImage(this FileInfoJB fileInfo)
        {
            return fileInfo.Extension.ToUpperInvariant() == ".JPEG"
                || fileInfo.Extension.ToUpperInvariant() == ".JPG"
                || fileInfo.Extension.ToUpperInvariant() == ".GIF"
                || fileInfo.Extension.ToUpperInvariant() == ".BMP";
        }
    }
}
