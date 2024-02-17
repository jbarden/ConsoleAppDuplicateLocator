using System;
using System.Diagnostics;
using System.IO;
using System.IO.Abstractions;

namespace ConsoleAppDuplicateLocator;

public partial class Program
{
    private const bool RecursiveSubDirectories = true;

    private static void Main()
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        StuffWithEvents.FilesEventHandler += FilesService_SelectPotentialDuplicatesEventHandler;
        var searchParameters = new SearchParameters("F:\\LookAtNow\\f\\", RecursiveSubDirectories, "*.*");
        StuffWithEvents.DoVeryImportantStuff(searchParameters, new FileSystem());
        StuffWithEvents.FilesEventHandler -= FilesService_SelectPotentialDuplicatesEventHandler;
        stopwatch.Stop();
        Console.WriteLine($"Total run time: {stopwatch.Elapsed.Minutes} minutes");

        File.AppendAllText(@"c:\temp\dups.txt", $"{Environment.NewLine}{DateTime.Now} Total run time: {stopwatch.Elapsed.Minutes} minutes{Environment.NewLine}");
        _ = Console.ReadKey();
    }

    private static void FilesService_SelectPotentialDuplicatesEventHandler(object? sender, EventArgs eventArgs)
        => Console.WriteLine(((FileEventArgs)eventArgs).Message);
}