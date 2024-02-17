using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppDuplicateLocator;

internal static class StringExtensions
{
    public static bool IsImage(this string name)
        => name.EndsWith("jpg", StringComparison.OrdinalIgnoreCase)
        || name.EndsWith("jpeg", StringComparison.OrdinalIgnoreCase)
        || name.EndsWith("bmp", StringComparison.OrdinalIgnoreCase)
        || name.EndsWith("gif", StringComparison.OrdinalIgnoreCase)
        || name.EndsWith("jfif", StringComparison.OrdinalIgnoreCase)
        || name.EndsWith("png", StringComparison.OrdinalIgnoreCase);

    public static bool IsNotImage(this string name)
        => !IsImage(name);
}
