using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;

namespace ConsoleAppDuplicateLocator;

internal static class StuffWithEvents
{
    public static EventHandler? FilesEventHandler;

    public static void DoVeryImportantStuff(SearchParameters searchParameters, IFileSystem fileSystem)
    {
        var fileList = new List<FileInfoJB>();
        RaiseEvent(new FileEventArgs("Getting the file details..."));

        GetFileList(searchParameters, fileSystem)
            .ForEach(file => fileList.Add(GetFileInfo(file)));

        RaiseEvent(new FileEventArgs("Grouping the file details..."));

        var duplicatesBySize = fileList
            .GroupBy(file => FileSize.Create(file.Size, file.Height, file.Width, file.ChecksumHash), new FileSizeEqualityComparer()).Where(files => files.Count() > 1)
            .ToArray();

        var duplicatesWithDimensions = new List<FileInfoJB>();
        foreach (var fileGroup in duplicatesBySize)
        {
            for (var i = 0; i < fileGroup.Count(); i++)
            {
                if (!fileGroup.ElementAt(i).IsImage)
                {
                    continue;
                }

                RaiseEvent(new FileEventArgs($"Getting the file dimension details for {fileGroup.ElementAt(i).Name}..."));

                var fileWithDimensions = GetFileInfo(fileGroup.ElementAt(i));
                duplicatesWithDimensions.Add(fileWithDimensions);
            }
        }

        RaiseEvent(new FileEventArgs("Grouping the file details again - this time with dimensions for the duplicates..."));
        var duplicatesBySizeAndDimensions = duplicatesWithDimensions
                                        .GroupBy(file => FileSize.Create(file.Size, file.Height, file.Width, file.ChecksumHash), new FileSizeEqualityComparer())
                                        .Where(files => files.Count() > 1);

        Console.WriteLine(new string('-', 40));
        Console.WriteLine($"files2 count: {fileList.Count}");
        Console.WriteLine($"Duplicate by size count: {duplicatesBySize.Count()}");
        Console.WriteLine($"Duplicate by size and dimensions count: {duplicatesBySizeAndDimensions.Count()}");

        Console.WriteLine(new string('-', 40));
        Console.WriteLine();
        Console.WriteLine("File Size\tFile Name");

        foreach (IGrouping<FileSize, FileInfoJB>? group in duplicatesBySizeAndDimensions)
        {
            var key = group.Key;
            Console.WriteLine(key.FileLength);
            foreach (var file in group)
            {
                Console.WriteLine("\t\t" + file.FullName);
            }
        }
    }

    private static List<FileInfo> GetFileList(SearchParameters searchParameters, IFileSystem fileSystem)
    {
        var files = fileSystem.Directory.GetFiles(searchParameters.SearchFolder, searchParameters.SearchPattern, new EnumerationOptions { IgnoreInaccessible = true, MatchCasing = MatchCasing.CaseInsensitive, RecurseSubdirectories = searchParameters.RecursiveSubDirectories });
        var fileList = new List<FileInfo>();

        foreach (var file in files)
        {
            RaiseEvent(new FileEventArgs($"Getting the file details for {file}..."));

            fileList.Add(new FileInfo(file));
        }

        return fileList;
    }

    private static FileInfoJB GetFileInfo(FileInfo file) => new() { FullName = file.FullName, Height = 0, Width = 0, Name = file.Name, Size = file.Length, Extension = file.Extension, ChecksumHash = string.Empty };

    private static FileInfoJB GetFileInfo(FileInfoJB file)
    {
        if (file.IsImage())
        {
            using var fsSource = new FileStream(file.FullName, FileMode.Open, FileAccess.Read);
            using var bitmap = SKBitmap.Decode(fsSource);

            var height = bitmap.Height;
            var width = bitmap.Width;

            return new FileInfoJB { FullName = file.FullName, Height = height, Width = width, Name = file.Name, Size = file.Size, Extension = file.Extension, ChecksumHash = string.Empty };
        }

        return file;
    }

    private static void RaiseEvent(FileEventArgs searchEventArgs) =>
        FilesEventHandler?.Invoke(null, searchEventArgs);
}