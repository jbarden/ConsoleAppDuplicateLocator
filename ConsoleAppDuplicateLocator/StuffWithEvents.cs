using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Drawing;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text.Json;

namespace ConsoleAppDuplicateLocator;

internal static class StuffWithEvents
{
    public static EventHandler? FilesEventHandler;

    public static void DoVeryImportantStuff(SearchParameters searchParameters, IFileSystem fileSystem)
    {
        var fileList = new ConcurrentBag<FileInfoJB>();
        RaiseEvent(new FileEventArgs($"Getting the file details for {searchParameters.SearchFolder} (including subdirectories: {searchParameters.RecursiveSubDirectories})..."));
        
        var counter = 0;
        GetFileList(searchParameters, fileSystem).AsParallel().ForAll(file => fileList.Add(GetFileInfo(new FileInfoJB
        {
            FullName = file.FullName,
            Size = file.Length,
            Name = file.Name,
            Extension = file.Extension
        }, counter++)));

        RaiseEvent(new FileEventArgs("Grouping the file details - this time with dimensions for the duplicates..."));
        var duplicatesBySizeAndDimensions = fileList
                                        .GroupBy(file => FileSize.Create(file.Size, file.Height, file.Width, file.ChecksumHash), new FileSizeEqualityComparer())
                                        .Where(files => files.Count() > 1).ToList();

        Console.WriteLine(new string('-', 40));
        Console.WriteLine($"files2 count: {fileList.Count}");
        Console.WriteLine($"Duplicate by size and dimensions count: {duplicatesBySizeAndDimensions.Count}");
        var dl = new List<string>();
        foreach (var test in duplicatesBySizeAndDimensions.Select(duplicatesBySizeAndDimension => duplicatesBySizeAndDimension.GetEnumerator()))
        {
            while (test.MoveNext())
            {
                var what = test.Current;
                var i = what.FullName.LastIndexOf(@"\", StringComparison.Ordinal)+1;
                var fullname = what.FullName[..i];
                if (!dl.Contains(fullname))
                {
                    dl.Add(fullname);
                }
            }
        }

        File.WriteAllText(@"c:\temp\dups.txt",JsonSerializer.Serialize(dl));
    }

    private static List<FileInfo> GetFileList(SearchParameters searchParameters, IFileSystem fileSystem)
    {
        var fileList = new List<FileInfo>();
        var files = fileSystem.Directory.GetFiles(searchParameters.SearchFolder, searchParameters.SearchPattern, new EnumerationOptions { IgnoreInaccessible = true, MatchCasing = MatchCasing.CaseInsensitive, RecurseSubdirectories = searchParameters.RecursiveSubDirectories}).ToList();
        var counter = 0;
        foreach (var file in files)
        {
            if (counter % 10 == 0)
            {
                RaiseEvent(new FileEventArgs($"{counter++} Getting the file details for {file}..."));
            }

            fileList.Add(new FileInfo(file));
        }

        return fileList;
    }

    private static FileInfoJB GetFileInfo(FileInfoJB file, int counter)
    {
        if(counter%10==0) {
            RaiseEvent(new FileEventArgs($"{counter} GetFileInfo(FileInfoJB) for {file.FullName}..."));
        }

        try
        {
            if (file.IsNotImage)
            {
                return file;
            }

            using var img = Image.FromFile(file.FullName);

            return new FileInfoJB { FullName = file.FullName, Height = img.Height, Width = img.Width, Name = file.Name, Size = file.Size, Extension = file.Extension, ChecksumHash = string.Empty };

        }
        catch
        {
            return new FileInfoJB();
        }
    }

    private static void RaiseEvent(EventArgs searchEventArgs) =>FilesEventHandler?.Invoke(null, searchEventArgs);
}