using System;
using System.Diagnostics;
using System.IO;
using System.IO.Abstractions;

namespace ConsoleAppDuplicateLocator;

public partial class Program
{
    private static readonly bool RecursiveSubDirectories = true;

    private static void Main(string[] args)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        StuffWithEvents.FilesEventHandler += FilesService_SelectPotentialDuplicatesEventHandler;
        var searchParameters = new SearchParameters("c:\\temp", RecursiveSubDirectories, "*.*");
        StuffWithEvents.DoVeryImportantStuff(searchParameters, new FileSystem());
        StuffWithEvents.FilesEventHandler -= FilesService_SelectPotentialDuplicatesEventHandler;
        stopwatch.Stop();
        Console.WriteLine($"Total run time: {stopwatch.ElapsedMilliseconds} milliseconds");

        File.AppendAllText(@"c:\temp\stuffWithEvents.txt", $"{DateTime.Now} Total run time: {stopwatch.ElapsedMilliseconds}{Environment.NewLine}");
        _ = Console.ReadKey();
    }

    private static void FilesService_SelectPotentialDuplicatesEventHandler(object? sender, EventArgs eventArgs)
        => Console.WriteLine(((FileEventArgs)eventArgs).Message);
}