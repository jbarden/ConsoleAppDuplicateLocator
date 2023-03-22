using System;

namespace ConsoleAppDuplicateLocator;

internal class FileInfoJB
{
    public string Name { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public long Height { get; set; }

    public long Width { get; set; }

    public long Size { get; set; }

    public string ChecksumHash { get; set; } = string.Empty;

    public string Extension { get; set; } = string.Empty;

    public bool IsImage => Extension.Contains(".jpg", StringComparison.OrdinalIgnoreCase)
                           || Extension.Contains(".bmp", StringComparison.OrdinalIgnoreCase)
                           || Extension.Contains(".png", StringComparison.OrdinalIgnoreCase);
}