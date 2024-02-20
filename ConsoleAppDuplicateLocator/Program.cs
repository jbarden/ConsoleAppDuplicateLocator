using System;
using System.IO.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace ConsoleAppDuplicateLocator;

public partial class Program
{
    private const bool RecursiveSubDirectories = true;

    static Program()
    {
        Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.Seq("http://localhost:5341")
    .CreateLogger();
    }

    private static void Main(string[] args)
    {
        var services = new ServiceCollection();
        _ = services.AddLogging(loggingBuilder =>
        {
            loggingBuilder.AddConsole();
            loggingBuilder.AddSeq();
        });
        var serviceProvider = services.BuildServiceProvider();
       var logger = serviceProvider.GetRequiredService<ILogger<SearchForDuplicates>>();
        var startTime = DateTime.Now;
        var stuff = new SearchForDuplicates(logger);
        var searchParameters = new SearchParameters(args[0], RecursiveSubDirectories, "*.*");
        stuff.DoVeryImportantStuff(searchParameters, new FileSystem(), args[1]);
        var elapsed = DateTime.Now - startTime;
        logger.LogInformation("Total run time: {Seconds} Seconds", elapsed.Seconds);
        Log.CloseAndFlush();
    }
}
