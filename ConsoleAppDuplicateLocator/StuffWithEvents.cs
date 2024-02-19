using Microsoft.Extensions.Logging;
using SkiaSharp;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace ConsoleAppDuplicateLocator;

internal class StuffWithEvents
{
    private readonly ILogger<StuffWithEvents> logger;

    public StuffWithEvents(ILogger<StuffWithEvents> logger)
    {
        this.logger = logger;
    }

    public EventHandler? FilesEventHandler;

    public void DoVeryImportantStuff(SearchParameters searchParameters, IFileSystem fileSystem, string logFile)
    {
        RaiseEvent(new FileEventArgs($"Getting the file details for {searchParameters.SearchFolder} (including subdirectories: {searchParameters.RecursiveSubDirectories})..."));

        var fileList = GetFileList(searchParameters, fileSystem);

        RaiseEvent(new FileEventArgs("Grouping the file details - this time only with the size for the duplicates..."));
        var duplicatesBySize = fileList
                                        .GroupBy(file => FileSize.Create(file.Size), new FileSizeEqualityComparer())
                                        .Where(files => files.Count() > 1).ToList();

        RaiseEvent(new FileEventArgs($"Getting the file dimensions for potential duplicates by size"));

        var duplicatesWithSizeAndDimensions = GetDimensionsForPotentialDuplicateImages(duplicatesBySize);

        RaiseEvent(new FileEventArgs("Grouping the file details - this time with dimensions for the duplicates..."));

        List< (FileSize FileSize, List<FileInfoJB> Files)> duplicates = new ();
        foreach (IGrouping<FileSize, FileInfoJB> group in duplicatesWithSizeAndDimensions)
        {
            var s= group.Key;
            var groupList = group.ToList();

           var dups=  groupList.GroupBy(file => FileSize.Create(file.Size, file.Height, file.Width), new FileSizeEqualityComparer())
                                        .Where(files => files.Count() > 1)
                                        .SelectMany(f=>f.ToList()).ToList();
            duplicates.Add(new (s, dups));
        }

        logger.LogInformation($"fileList count: {fileList.Count}");
        logger.LogInformation($"Duplicate by size: {duplicatesBySize.Count}");
        logger.LogInformation($"Duplicate by size and dimensions count: {duplicates.Count}");
        fileSystem.File.Delete(logFile);
        foreach((FileSize FileSize, List<FileInfoJB> Files) duplicateGroup in duplicates)
        {
            logger.LogInformation("File Size: {fileSize}, file count: {count}", duplicateGroup.FileSize.FileLength.ToString(), duplicateGroup.Files.Count());
            File.AppendAllText(logFile, $"File Size: {duplicateGroup.FileSize.FileLength}. File Count: {duplicateGroup.Files.Count()}");
            foreach (var file in duplicateGroup.Files)
            {
                logger.LogInformation("    File name: {fileName}", file.FullName);
                File.WriteAllText(logFile, $"\t{file.FullName}");
            }
        }
    }

    private List<IGrouping<FileSize, FileInfoJB>> GetDimensionsForPotentialDuplicateImages(List<IGrouping<FileSize, FileInfoJB>> duplicatesBySize)
    {
        foreach(var fileGroup in duplicatesBySize)
        {
            var test = fileGroup.GetEnumerator();
            while (test.MoveNext())
            {
                var file = test.Current;
                if(file.IsImage())
                {
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
        logger.LogInformation("Start Time: {Time}", startTime);
        var fileList = new ConcurrentBag<FileInfoJB>();
        var enumerationOptions = new EnumerationOptions
        {
            IgnoreInaccessible = true,
            MatchCasing = MatchCasing.CaseInsensitive,
            RecurseSubdirectories = searchParameters.RecursiveSubDirectories
        };
        var files = fileSystem.Directory.GetFiles(searchParameters.SearchFolder, searchParameters.SearchPattern, enumerationOptions).AsParallel();
        logger.LogInformation("GetFiles Start Time: {Time} {Milliseconds}", startTime, startTime.Millisecond);
        logger.LogInformation("GetFiles End Time: {Time} {Milliseconds}", DateTime.Now, DateTime.Now.Millisecond);
        var counter = 0;
        foreach (var file in files.AsParallel())
        {
            if (counter % 10 == 0)
            {
                RaiseEvent(new FileEventArgs($"Getting the file details for {file}..."));
            }

            try
            {
                counter++;
                var fileInfo = new FileInfo(file);

                fileList.Add(new (fileInfo));
            }
            catch
            {
                // NAR at the moment
            }
        }

        var elapsed = DateTime.Now - startTime;
        logger.LogInformation("GetFileList Total run time: {Seconds} Seconds", elapsed.Seconds);
        logger.LogInformation("Start Time: {Time}", startTime);
        logger.LogInformation("End Time: {Time}", DateTime.Now);

        return fileList;
    }

    private void RaiseEvent(EventArgs searchEventArgs) => FilesEventHandler?.Invoke(null, searchEventArgs);
}
