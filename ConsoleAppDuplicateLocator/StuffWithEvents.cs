using System;
using System.Collections.Generic;
using System.Drawing;
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

        GetFileList(searchParameters/*, fileSystem*/)
            .ForEach(file => fileList.Add(GetFileInfo(file)));

        RaiseEvent(new FileEventArgs("Grouping the file details..."));

        var dupsBySize = fileList.GroupBy(file => FileSize.Create(file.Size, file.Height, file.Width, file.ChecksumHash), new FileSizeEqualityComparer()).Where(files => files.Count() > 1);

        var dupsWithDimensions = new List<FileInfoJB>();
        foreach (var fileGroup in dupsBySize)
        {
            for (var i = 0; i < fileGroup.Count(); i++)
            {
                if (fileGroup.ElementAt(i).IsImage)
                {
                    if (i % searchParameters.EventRaiseCounter == 0)
                    {
                        RaiseEvent(new FileEventArgs($"Getting the file dimension details for {fileGroup.ElementAt(i).Name}..."));
                    }

                    var fileWithDimensions = GetFileInfo(fileGroup.ElementAt(i));
                    dupsWithDimensions.Add(fileWithDimensions);
                }
            }
        }

        RaiseEvent(new FileEventArgs("Grouping the file details again - this time with dimensions for the duplicates..."));
        var dupsBySizeAndDimensions = dupsWithDimensions
                                        .GroupBy(file => FileSize.Create(file.Size, file.Height, file.Width, file.ChecksumHash), new FileSizeEqualityComparer())
                                        .Where(files => files.Count() > 1);

        Console.WriteLine(new string('-', 40));
        Console.WriteLine($"files2 count: {fileList.Count}");
        Console.WriteLine($"Duplicate by size count: {dupsBySize.Count()}");
        Console.WriteLine($"Duplicate by size and dimensions count: {dupsBySizeAndDimensions.Count()}");
    }

    private static List<FileInfo> GetFileList(SearchParameters searchParameters)
    {
        //var files = fileSystem.Directory.GetFiles(searchParameters.SearchFolder, searchParameters.SearchPattern, new EnumerationOptions { IgnoreInaccessible = true, MatchCasing = MatchCasing.CaseInsensitive, RecurseSubdirectories = searchParameters.RecurseSubdirectories });
        var fileList = new List<FileInfo>();
        var files = new List<string>
        {
            @"c:\temp\files.txt",
            @"c:\temp\09_Edp.png",
            @"c:\temp\10.jpg",
            @"c:\temp\2018-apollo-intensa-emozione-launch (1).jpg",
            @"c:\temp\2018-apollo-intensa-emozione-launch.jpg",
            @"c:\temp\2880-1800-crop-lamborghini-countach-25th-anniversary-c280212122018184243_1.jpg",
            @"c:\temp\45f69f5a-lamborghini-sian-2.jpg",
            @"c:\temp\4B1B0F1C-7310-45C7-9246-2AD85B2A9263.jpg",
            @"c:\temp\4dd8ba95-lamborghini-sian-design.jpg",
            @"c:\temp\723E62E5-818E-40F7-9E4C-97EC10D61276.jpg",
            @"c:\temp\72B08573-1A16-44BE-9EA3-1A360E8D54D8.jpg",
            @"c:\temp\96F1C3FB-F572-4A21-87C5-9ACCBC3BCED5.jpg",
            @"c:\temp\New Folder\09_Edp.png",
            @"c:\temp\New Folder\10.jpg",
            @"c:\temp\New Folder\2018-apollo-intensa-emozione-launch (1).jpg",
            @"c:\temp\New Folder\2018-apollo-intensa-emozione-launch.jpg",
            @"c:\temp\New Folder\2880-1800-crop-lamborghini-countach-25th-anniversary-c280212122018184243_1.jpg",
            @"c:\temp\New Folder\45f69f5a-lamborghini-sian-2.jpg",
            @"c:\temp\New Folder\4B1B0F1C-7310-45C7-9246-2AD85B2A9263.jpg",
            @"c:\temp\New Folder\4dd8ba95-lamborghini-sian-design.jpg",
            @"c:\temp\New Folder\723E62E5-818E-40F7-9E4C-97EC10D61276.jpg",
            @"c:\temp\New Folder\72B08573-1A16-44BE-9EA3-1A360E8D54D8.jpg",
            @"c:\temp\New Folder\96F1C3FB-F572-4A21-87C5-9ACCBC3BCED5.jpg",
            @"c:\temp\New Folder - Copy\09_Edp.png",
            @"c:\temp\New Folder - Copy\10.jpg",
            @"c:\temp\New Folder - Copy\2018-apollo-intensa-emozione-launch (1).jpg",
            @"c:\temp\New Folder - Copy\2018-apollo-intensa-emozione-launch.jpg",
            @"c:\temp\New Folder - Copy\2880-1800-crop-lamborghini-countach-25th-anniversary-c280212122018184243_1.jpg",
            @"c:\temp\New Folder - Copy\45f69f5a-lamborghini-sian-2.jpg",
            @"c:\temp\New Folder - Copy\4B1B0F1C-7310-45C7-9246-2AD85B2A9263.jpg",
            @"c:\temp\New Folder - Copy\4dd8ba95-lamborghini-sian-design.jpg",
            @"c:\temp\New Folder - Copy\723E62E5-818E-40F7-9E4C-97EC10D61276.jpg",
            @"c:\temp\New Folder - Copy\72B08573-1A16-44BE-9EA3-1A360E8D54D8.jpg",
            @"c:\temp\New Folder - Copy\96F1C3FB-F572-4A21-87C5-9ACCBC3BCED5.jpg",
        };
        var fileCounter = 1;
        foreach (var file in files)
        {
            if (fileCounter % searchParameters.EventRaiseCounter == 0)
            {
                RaiseEvent(new FileEventArgs($"Getting the file details for {file}..."));
            }

            fileList.Add(new FileInfo(file));
            fileCounter++;
        }

        return fileList;
    }

    private static FileInfoJB GetFileInfo(FileInfo file) => new() { FullName = file.FullName, Height = 0, Width = 0, Name = file.Name, Size = file.Length, Extension = file.Extension, ChecksumHash = string.Empty };

    private static FileInfoJB GetFileInfo(FileInfoJB file)
    {
        if (file.IsImage())
        {
            using var img = Image.FromFile(file.FullName);

            return new FileInfoJB { FullName = file.FullName, Height = img.Height, Width = img.Width, Name = file.Name, Size = file.Size, Extension = file.Extension, ChecksumHash = string.Empty };
        }

        return file;
    }

    private static void RaiseEvent(FileEventArgs searchEventArgs) =>
        FilesEventHandler?.Invoke(null, searchEventArgs);
}