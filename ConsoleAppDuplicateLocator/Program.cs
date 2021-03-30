using System;
using System.Diagnostics;
using System.IO;
using System.IO.Abstractions;

namespace ConsoleAppDuplicateLocator
{
    public partial class Program
    {
        private static readonly bool RecurseSubdirectories = true;

        private static void Main(string[] args)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            StuffWithEvents.FilesEventHandler += FilesService_SelectPotentialDuplicatesEventHandler;
            var searchParameters = new SearchParameters(10, "c:\\temp", RecurseSubdirectories, "*.*");
            StuffWithEvents.DoVeryImportantStuff(searchParameters, new FileSystem());
            StuffWithEvents.FilesEventHandler -= FilesService_SelectPotentialDuplicatesEventHandler;
            stopwatch.Stop();
            Console.WriteLine($"Total run time: {stopwatch.ElapsedMilliseconds} milliseconds");

            File.AppendAllText(@"c:\temp\stuffWithEvents.txt", $"{DateTime.Now} Total run time: {stopwatch.ElapsedMilliseconds}{Environment.NewLine}");
        }

        private static void OldWayWithInputs()
        {
            Console.WriteLine("Hello World!");
            Console.WriteLine("Please Select '1' for normal stuff, and '2' for events");
            Console.WriteLine("'1' for normal stuff");
            Console.WriteLine("'2' for events-based stuff");
            var response = Console.ReadKey();

            for (int i = 0; i < 1; i++)
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                if (response.Key == ConsoleKey.D1 || response.Key == ConsoleKey.NumPad1)
                {
                    Stuff.DoVeryImportantStuff();
                }
                if (response.Key == ConsoleKey.D2 || response.Key == ConsoleKey.NumPad2)
                {
                    StuffWithEvents.FilesEventHandler += FilesService_SelectPotentialDuplicatesEventHandler;
                    StuffWithEvents.DoVeryImportantStuff(new SearchParameters(10, "c:\\temp", RecurseSubdirectories, "*.*"), new FileSystem());
                    StuffWithEvents.FilesEventHandler -= FilesService_SelectPotentialDuplicatesEventHandler;
                }
                stopwatch.Stop();
                Console.WriteLine($"Total run time: {stopwatch.ElapsedMilliseconds} milliseconds");

                if (response.Key == ConsoleKey.D1 || response.Key == ConsoleKey.NumPad1)
                {
                    File.AppendAllText(@"c:\temp\stuff.txt", $"{DateTime.Now} - Total run time: {stopwatch.ElapsedMilliseconds}{Environment.NewLine}");
                }
                if (response.Key == ConsoleKey.D2 || response.Key == ConsoleKey.NumPad2)
                {
                    File.AppendAllText(@"c:\temp\stuffWithEvents.txt", $"{DateTime.Now} Total run time: {stopwatch.ElapsedMilliseconds}{Environment.NewLine}");
                }
            }
        }

        private static void FilesService_SelectPotentialDuplicatesEventHandler(object? sender, EventArgs eventArgs)
        {
            Console.WriteLine(((FileEventArgs)eventArgs).Message);
        }
    }
}