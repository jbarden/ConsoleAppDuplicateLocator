using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using Microsoft.Extensions.Logging;
using Serilog;
using SkiaSharp;

namespace ConsoleAppDuplicateLocator;

internal class SearchForDuplicates(ILogger<SearchForDuplicates> logger)
{
    public void DoVeryImportantStuff(SearchParameters searchParameters, IFileSystem fileSystem, string logFile)
    {
        Log.Information($"Getting the file details for {searchParameters.SearchFolder} (including subdirectories: {searchParameters.RecursiveSubDirectories})...");

        var fileList = GetFileList(searchParameters, fileSystem);

        Log.Information("Grouping the file details - this time only with the size for the duplicates...");
        var duplicatesBySize = fileList
                                        .GroupBy(file => FileSize.Create(file.Size), new FileSizeEqualityComparer())
                                        .Where(files => files.Count() > 1).ToList();

        Log.Information($"Getting the file dimensions for potential duplicates by size");

        var duplicatesWithSizeAndDimensions = GetDimensionsForPotentialDuplicateImages(duplicatesBySize);

        Log.Information("Grouping the file details - this time with dimensions for the duplicates...");

        List<(FileSize FileSize, List<FileInfoJB> Files)> duplicates = [];
        foreach (IGrouping<FileSize, FileInfoJB> group in duplicatesWithSizeAndDimensions.AsParallel())
        {
            var s = group.Key;
            var groupList = group.ToList();

            var dups = groupList.GroupBy(file => FileSize.Create(file.Size, file.Height, file.Width), new FileSizeEqualityComparer())
                                         .Where(files => files.Count() > 1)
                                         .SelectMany(f => f.ToList()).ToList();
            duplicates.Add(new(s, dups));
        }

        logger.LogInformation($"logger.LogInformation fileList count: {fileList.Count}");
        logger.LogInformation($"logger.LogInformation Duplicate by size: {duplicatesBySize.Count}");
        logger.LogInformation($"logger.LogInformation Duplicate by size and dimensions count: {duplicates.Count}");
        Log.Information($"fileList count: {fileList.Count}");
        Log.Information($"Duplicate by size: {duplicatesBySize.Count}");
        Log.Information($"Duplicate by size and dimensions count: {duplicates.Count}");
        fileSystem.File.Delete(logFile);
        foreach ((FileSize FileSize, List<FileInfoJB> Files) duplicateGroup in duplicates)
        {
            logger.LogInformation("logger File Size: {fileSize}, file count: {count}", duplicateGroup.FileSize.FileLength.ToString(), duplicateGroup.Files.Count());
            Log.Information("Log File Size: {fileSize}, file count: {count}", duplicateGroup.FileSize.FileLength.ToString(), duplicateGroup.Files.Count());
            foreach (var file in duplicateGroup.Files)
            {
                logger.LogInformation("  logger  File name: {fileName}", file.FullName);
                Log.Information("  Log  File name: {fileName}", file.FullName);
            }
        }

        Log.Information("Done");
    }

    private List<IGrouping<FileSize, FileInfoJB>> GetDimensionsForPotentialDuplicateImages(List<IGrouping<FileSize, FileInfoJB>> duplicatesBySize)
    {
        var counter = 0;
        foreach (var fileGroup in duplicatesBySize)
        {
            var test = fileGroup.GetEnumerator();
            while (test.MoveNext())
            {
                var file = test.Current;
                if (file.IsImage())
                {
                    if (counter % 10 == 0)
                    {
                        Log.Information("Counter: {counter}, Getting the file dimensions for {file}", counter, file.FullName);
                    }

                    counter++;
                    using var image = SKImage.FromEncodedData(file.FullName);
                    using var bm = SKBitmap.FromImage(image);

                    file.Width = bm.Width;
                    file.Height = bm.Height;
                }
            }
        }

        return duplicatesBySize;
    }

    private ConcurrentBag<FileInfoJB> GetFileList(SearchParameters searchParameters, IFileSystem fileSystem)
    {
        var startTime = DateTime.Now;
        Log.Information("Start Time: {Time}", startTime);
        var fileList = new ConcurrentBag<FileInfoJB>();
        var enumerationOptions = new EnumerationOptions
        {
            IgnoreInaccessible = true,
            MatchCasing = MatchCasing.CaseInsensitive,
            RecurseSubdirectories = searchParameters.RecursiveSubDirectories
        };
        var files = fileSystem.Directory.GetFiles(searchParameters.SearchFolder, searchParameters.SearchPattern, enumerationOptions).AsParallel();
        Log.Information("GetFiles Start Time: {Time} {Milliseconds}", startTime, startTime.Millisecond);
        Log.Information("GetFiles End Time: {Time} {Milliseconds}", DateTime.Now, DateTime.Now.Millisecond);
        var counter = 0;
        foreach (var file in files.AsParallel())
        {
            if (counter % 10 == 0)
            {
                Log.Information("Counter: {counter}, Getting the file details for {file}", counter, file);
            }

            try
            {
                counter++;
                var fileInfo = new FileInfo(file);

                fileList.Add(new(fileInfo));
            }
            catch
            {
                // NAR at the moment
            }
        }

        var elapsed = DateTime.Now - startTime;
        Log.Information("GetFileList Total run time: {Seconds} Seconds", elapsed.Seconds);
        Log.Information("Start Time: {Time}", startTime);
        Log.Information("End Time: {Time}", DateTime.Now);

        return fileList;
    }
}
