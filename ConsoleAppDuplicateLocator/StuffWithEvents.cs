using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Abstractions;
using System.Linq;

namespace ConsoleAppDuplicateLocator
{
    internal static class StuffWithEvents
    {
        public static EventHandler? FilesEventHandler;

        public static void DoVeryImportantStuff(SearchParameters searchParameters, IFileSystem fileSystem)
        {
            var fileList = new List<FileInfoJB>();
            RaiseEvent(new FileEventArgs("Getting the file details..."));

            GetFileList(searchParameters)
                .ForEach(file => fileList.Add(GetFileInfo(file)));

            RaiseEvent(new FileEventArgs("Grouping the file details..."));

            var dupsBySize = fileList.GroupBy(file => FileSize.Create(file.Size, file.Height, file.Width, file.ChecksumHash), new FileSizeEqualityComparer()).Where(files => files.Count() > 1);

            var dupsWithDimensions = new List<FileInfoJB>();
            foreach (var fileGroup in dupsBySize)
            {
                for (int i = 0; i < fileGroup.Count(); i++)
                {
                    if (fileGroup.ElementAt(i).Extension.Contains(".jpg", StringComparison.OrdinalIgnoreCase))
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
            var dupsBySizeAndDimensions = dupsWithDimensions.GroupBy(file => FileSize.Create(file.Size, file.Height, file.Width, file.ChecksumHash), new FileSizeEqualityComparer()).Where(files => files.Count() > 1);

            Console.WriteLine(new string('-', 40));
            Console.WriteLine($"files2 count: {fileList.Count}");
            Console.WriteLine($"Duplicate by size count: {dupsBySize.Count()}");
            Console.WriteLine($"Duplicate by size and dimensions count: {dupsBySizeAndDimensions.Count()}");
        }

        private static List<FileInfo> GetFileList(SearchParameters searchParameters)
        {
            var files = Directory.GetFiles(searchParameters.SearchFolder, searchParameters.SearchPattern, new EnumerationOptions { IgnoreInaccessible = true, MatchCasing = MatchCasing.CaseInsensitive, RecurseSubdirectories = searchParameters.RecurseSubdirectories });
            var fileList = new List<FileInfo>();

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

        private static FileInfoJB GetFileInfo(FileInfo file)
        {
            return new FileInfoJB { FullName = file.FullName, Height = 0, Width = 0, Name = file.Name, Size = file.Length, Extension = file.Extension, ChecksumHash = string.Empty };
        }

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
}