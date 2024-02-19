using System;
using System.Diagnostics;
using System.IO.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ConsoleAppDuplicateLocator;

public partial class Program
{
    private const bool RecursiveSubDirectories = true;
    private static readonly ILogger<StuffWithEvents> StuffWithEventsLogger;

    static Program()
    {
        var services = new ServiceCollection();
        _ = services.AddLogging(logging => _ = logging.AddConsole());
        var serviceProvider = services.BuildServiceProvider();
        StuffWithEventsLogger = serviceProvider.GetRequiredService<ILogger<StuffWithEvents>>();
        Console.ReadKey();
    }

    private static void Main(string[] args)
    {
        var services = new ServiceCollection();
        _ = services.AddLogging(logging => _ = logging.AddConsole());
        var serviceProvider = services.BuildServiceProvider();
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

        var startTime = DateTime.Now;
        var stuff = new StuffWithEvents(serviceProvider.GetRequiredService<ILogger<StuffWithEvents>>());
        stuff.FilesEventHandler += FilesService_SelectPotentialDuplicatesEventHandler;
        var searchParameters = new SearchParameters(args[0], RecursiveSubDirectories, "*.*");
        stuff.DoVeryImportantStuff(searchParameters, new FileSystem(), args[1]);
        stuff.FilesEventHandler -= FilesService_SelectPotentialDuplicatesEventHandler;
        var elapsed = DateTime.Now - startTime;
        logger.LogInformation("Total run time: {Seconds} Seconds", elapsed.Seconds);
    }

    private static void FilesService_SelectPotentialDuplicatesEventHandler(object? sender, EventArgs eventArgs)
        => StuffWithEventsLogger.LogInformation("Message: {message}", ((FileEventArgs)eventArgs).Message);
}
