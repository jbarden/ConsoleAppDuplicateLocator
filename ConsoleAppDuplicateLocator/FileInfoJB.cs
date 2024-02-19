using System;
using System.IO;

namespace ConsoleAppDuplicateLocator;

internal class FileInfoJB
{
    public string Name
    {
        get
        {
            var index = FullName.LastIndexOf(@"\") + 1;
            return index >= 1 ? FullName[index..] : string.Empty;
        }
    }

    public FileInfoJB(FileInfo fileInfo)
    {
        FullName= fileInfo.FullName;
        Size = fileInfo.Length;
        Extension = fileInfo.Extension;
    }

    public string FullName { get; }

    public long Height { get; set; }

    public long Width { get; set; }

    public long Size { get; }

    public string Extension { get; }

    public bool IsNotImage => !IsImage;

    public bool IsImage => Extension.Contains(".jpg", StringComparison.OrdinalIgnoreCase)
                           || Extension.Contains(".bmp", StringComparison.OrdinalIgnoreCase)
                           || Extension.Contains(".png", StringComparison.OrdinalIgnoreCase);
}
