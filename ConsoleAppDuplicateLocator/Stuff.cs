using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace ConsoleAppDuplicateLocator
{
    internal static class Stuff
    {
        public static void DoVeryImportantStuff()
        {
            var fileList = new List<FileInfoJB>();

            GetFileList().ForEach(file => fileList.Add(GetFileInfo(file)));

            var dupsBySize = fileList.GroupBy(file => FileSize.Create(file.Size, file.Height, file.Width, file.ChecksumHash), new FileSizeEqualityComparer()).Where(files => files.Count() > 1);

            var dupsWithDimensions = new List<FileInfoJB>();
            foreach (var fileGroup in dupsBySize)
            {
                for (int i = 0; i < fileGroup.Count(); i++)
                {
                    var fileWithDimensions = GetFileInfo(fileGroup.ElementAt(i));
                    dupsWithDimensions.Add(fileWithDimensions);
                }
            }

            var dupsBySizeAndDimensions = dupsWithDimensions.GroupBy(file => FileSize.Create(file.Size, file.Height, file.Width, file.ChecksumHash), new FileSizeEqualityComparer()).Where(files => files.Count() > 1);

            Console.WriteLine(new string('-', 40));
            Console.WriteLine($"files2 count: {fileList.Count}");
            Console.WriteLine($"Duplicate by size count: {dupsBySize.Count()}");
            Console.WriteLine($"Duplicate by size and dimensions count: {dupsBySizeAndDimensions.Count()}");
        }

        private static List<FileInfo> GetFileList()
        {
            var files = Directory.GetFiles(@"c:\temp", "*.*", new EnumerationOptions { IgnoreInaccessible = true, MatchCasing = MatchCasing.CaseInsensitive, RecurseSubdirectories = true });
            var fileList = new List<FileInfo>();

            foreach (var file in files)
            {
                fileList.Add(new FileInfo(file));
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
                long height = img.Height;
                long width = img.Width;
                return new FileInfoJB { FullName = file.FullName, Height = height, Width = width, Name = file.Name, Size = file.Size, Extension = file.Extension, ChecksumHash = string.Empty };

            }

            return file;
        }
    }
}
