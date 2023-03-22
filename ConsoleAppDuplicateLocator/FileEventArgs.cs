using System;

namespace ConsoleAppDuplicateLocator;

internal class FileEventArgs : EventArgs
{
    public FileEventArgs(string message)
    {
        Message = message;
    }

    public string Message { get; set; }
}