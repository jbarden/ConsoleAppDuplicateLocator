namespace ConsoleAppDuplicateLocator;

internal static class FileInfoJbExtensions
{
    public static bool IsImage(this FileInfoJB fileInfo)
    {
        return fileInfo.Extension.ToUpperInvariant() is ".JPEG"
               or ".JPG"
               or ".GIF"
               or ".BMP";
    }
}