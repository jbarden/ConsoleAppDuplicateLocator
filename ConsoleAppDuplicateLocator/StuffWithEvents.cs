using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text.Json;
using System.Threading;
using Microsoft.Extensions.Logging;
using SkiaSharp;

namespace ConsoleAppDuplicateLocator;

internal class StuffWithEvents
{
    private readonly ILogger<StuffWithEvents> logger;

    public StuffWithEvents(ILogger<StuffWithEvents> logger)
    {
        this.logger = logger;
    }

    public EventHandler? FilesEventHandler;

    public void DoVeryImportantStuff(SearchParameters searchParameters, IFileSystem fileSystem, string logFile, System.Diagnostics.Stopwatch stopwatch)
    {
        RaiseEvent(new FileEventArgs($"Getting the file details for {searchParameters.SearchFolder} (including subdirectories: {searchParameters.RecursiveSubDirectories})..."));

        var fileList = GetFileList(searchParameters, fileSystem, stopwatch);

        RaiseEvent(new FileEventArgs("Grouping the file details - this time with dimensions for the duplicates..."));
        var duplicatesBySizeAndDimensions = fileList
                                        .GroupBy(file => FileSize.Create(file.Size, file.Height, file.Width), new FileSizeEqualityComparer())
                                        .Where(files => files.Count() > 1).ToList();

        logger.LogInformation($"files2 count: {fileList.Count}");
        logger.LogInformation($"Duplicate by size and dimensions count: {duplicatesBySizeAndDimensions.Count}");
        var dl = new List<string>();
        foreach (var test in duplicatesBySizeAndDimensions.Select(duplicatesBySizeAndDimension => duplicatesBySizeAndDimension.GetEnumerator()))
        {
            while (test.MoveNext())
            {
                var what = test.Current;
                var i = what.FullName.LastIndexOf(@"\", StringComparison.Ordinal) + 1;
                var fullname = what.FullName[..i];
                if (!dl.Contains(fullname))
                {
                    dl.Add(fullname);
                }
            }
        }

        File.WriteAllText(logFile, JsonSerializer.Serialize(dl));
    }

    private ConcurrentBag<FileInfoJB> GetFileList(SearchParameters searchParameters, IFileSystem fileSystem, System.Diagnostics.Stopwatch stopwatch)
    {
        var fileList = new ConcurrentBag<FileInfoJB>();
        var enumerationOptions = new EnumerationOptions
        {
            IgnoreInaccessible = true,
            MatchCasing = MatchCasing.CaseInsensitive,
            RecurseSubdirectories = searchParameters.RecursiveSubDirectories
        };
        var files = fileSystem.Directory.GetFiles(searchParameters.SearchFolder, searchParameters.SearchPattern, enumerationOptions);
        var counter = 0;
        foreach (var file in files)
        {
            if (counter % 10 == 0)
            {
                RaiseEvent(new FileEventArgs($"Getting the file details for {file}..."));
            }

            try
            {
                Interlocked.Add(ref counter, 1);
                var extensionIndex = file.LastIndexOf(".") + 1;
                var extension = file[extensionIndex..];
                var fileInfo = new FileInfo(file);
                if (extension.IsNotImage())
                {
                    fileList.Add(new FileInfoJB
                    {
                        FullName = file,
                        Size = fileInfo.Length,
                        Extension = extension
                    });
                    continue;
                }

                using var image = SKImage.FromEncodedData(file);
                using var bm = SKBitmap.FromImage(image);

                fileList.Add(new FileInfoJB { FullName = file, Height = bm.Height, Width = bm.Width, Size = fileInfo.Length, Extension = extension });
            }
            catch
            {
                // NAR at the moment
            }
        }

        logger.LogInformation("Run time so far: {Seconds}", stopwatch.Elapsed.Seconds);

        return fileList;
    }

    private void RaiseEvent(EventArgs searchEventArgs) => FilesEventHandler?.Invoke(null, searchEventArgs);
}
