using System;
using System.IO;
using System.Security.Cryptography;

namespace ConsoleAppDuplicateLocator
{
    class MightWantAgain
    {

        private string GetHash(string fileName)
        {
            using var cryptoProvider = new SHA256Managed();
            return BitConverter.ToString(cryptoProvider.ComputeHash(File.Open(fileName, FileMode.Open)));
        }

        private void YouNeverKnow()
        {
            //var groupCount = 0;
            //foreach (var fileGroup in dupsBySize)
            //{
            //    Console.WriteLine($"Group {++groupCount}");
            //    for (int i = 0; i < fileGroup.Count(); i++)
            //    {
            //        var file = fileGroup.ElementAt(i);
            //        Console.WriteLine($"    Name: {file.FullName}, File Name: {file.Name}, Width: {file.Width}, Height: {file.Height}, Size: {file.Size}, Hash: {file.ChecksumHash}");
            //    }
            //}

            //Console.WriteLine(new string('-', 20));
            //Console.WriteLine($"Groups to select: {groupsToSelect}");
            //Console.WriteLine($"pagesToSkip: {pagesToSkip}");

            //groupCount = 0;
            //foreach (var fileGroup in dupsBySize.Skip(pagesToSkip * groupsToSelect).Take(groupsToSelect))
            //{
            //    Console.WriteLine($"Group {++groupCount}");
            //    for (int i = 0; i < fileGroup.Count(); i++)
            //    {
            //        var file = fileGroup.ElementAt(i);
            //        Console.WriteLine($"    Name: {file.FullName}, File Name: {file.Name}, Width: {file.Width}, Height: {file.Height}, Size: {file.Size}, Hash: {file.ChecksumHash}");
            //    }
            //}

            //groupCount = 0;
            //Console.WriteLine(new string('-', 20));
            //pagesToSkip++;
            //Console.WriteLine($"Groups to select: {groupsToSelect}");
            //Console.WriteLine($"pagesToSkip: {pagesToSkip}");
            //foreach (var fileGroup in dupsBySize.Skip(pagesToSkip * groupsToSelect).Take(groupsToSelect))
            //{
            //    Console.WriteLine($"Group {++groupCount}");
            //    for (int i = 0; i < fileGroup.Count(); i++)
            //    {
            //        var file = fileGroup.ElementAt(i);
            //        Console.WriteLine($"    Name: {file.FullName}, File Name: {file.Name}, Width: {file.Width}, Height: {file.Height}, Size: {file.Size}, Hash: {file.ChecksumHash}");
            //    }
            //}

            //groupCount = 0;
            //Console.WriteLine(new string('-', 20));
            //pagesToSkip++;
            //Console.WriteLine($"Groups to select: {groupsToSelect}");
            //Console.WriteLine($"pagesToSkip: {pagesToSkip}");
            //foreach (var fileGroup in dupsBySize.Skip(pagesToSkip * groupsToSelect).Take(groupsToSelect))
            //{
            //    Console.WriteLine($"Group {++groupCount}");
            //    for (int i = 0; i < fileGroup.Count(); i++)
            //    {
            //        var file = fileGroup.ElementAt(i);
            //        Console.WriteLine($"    Name: {file.FullName}, File Name: {file.Name}, Width: {file.Width}, Height: {file.Height}, Size: {file.Size}, Hash: {file.ChecksumHash}");
            //    }
            //}
            //Console.WriteLine(new string('-', 40));
            //foreach (var fileGroup in dupsBySize.Skip(pagesToSkip * groupsToSelect).Take(1))
            //{
            //    Console.WriteLine($"Group {++groupCount}");
            //    for (int i = 0; i < fileGroup.Count(); i++)
            //    {
            //        var file = fileGroup.ElementAt(i);
            //        Console.WriteLine($"    Name: {file.FullName}, File Name: {file.Name}, Width: {file.Width}, Height: {file.Height}, Size: {file.Size}, Hash: {file.ChecksumHash}");
            //    }
            //}
        }
    }
}
