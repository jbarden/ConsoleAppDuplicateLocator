using System.IO;

namespace ConsoleAppDuplicateLocator;

internal static class FileInfoExtensions
{
    public static bool IsImage(this FileInfo fileInfo)
    {
        return fileInfo.Extension.ToUpperInvariant() is ".JPEG"
               or ".JPG"
               or ".GIF"
               or ".BMP";
    }
}