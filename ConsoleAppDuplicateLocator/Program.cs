using System;
using System.Diagnostics;
using System.IO;
using System.IO.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ConsoleAppDuplicateLocator;

public partial class Program
{
    private const bool RecursiveSubDirectories = true;
    private static ILogger<StuffWithEvents> StuffWithEventsLogger = null!;

    static Program()
    {
        var services = new ServiceCollection();
        services.AddLogging(logging =>
        {
            _ = logging.AddConsole();
        });
        var serviceProvider = services.BuildServiceProvider();
        StuffWithEventsLogger = serviceProvider.GetRequiredService<ILogger<StuffWithEvents>>();
    }

    private static void Main(string[] args)
    {
        var services = new ServiceCollection();
        services.AddLogging(logging =>
        {
            _ = logging.AddConsole();
        });
        var serviceProvider = services.BuildServiceProvider();
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

        var stopwatch = new Stopwatch();
        stopwatch.Start();
        var stuff = new StuffWithEvents(serviceProvider.GetRequiredService<ILogger<StuffWithEvents>>());
        stuff.FilesEventHandler += FilesService_SelectPotentialDuplicatesEventHandler;
        var searchParameters = new SearchParameters(args[0], RecursiveSubDirectories, "*.*");
        stuff.DoVeryImportantStuff(searchParameters, new FileSystem(), args[1], stopwatch);
        stuff.FilesEventHandler -= FilesService_SelectPotentialDuplicatesEventHandler;
        stopwatch.Stop();
        logger.LogInformation("Total run time: {Seconds} Seconds", stopwatch.Elapsed.Seconds);
    }

    private static void FilesService_SelectPotentialDuplicatesEventHandler(object? sender, EventArgs eventArgs)
        => StuffWithEventsLogger.LogInformation("Message: {message}", ((FileEventArgs)eventArgs).Message);
}
