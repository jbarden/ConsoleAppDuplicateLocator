using System.IO.Abstractions;

namespace TestProject1.MoveToRealProject;

internal class DuplicateFileFinder(IFileSystem fileSystem)
{
    internal IEnumerable<string> GetInitialFilesMatchingSearchCriteria(SearchParameters searchParameters)
    {
        var enumerationOptions = new EnumerationOptions
        {
            IgnoreInaccessible = true,
            MatchCasing = MatchCasing.CaseInsensitive,
            RecurseSubdirectories = searchParameters.RecursiveSubDirectories
        };
        var fileList = fileSystem.Directory.GetFiles(searchParameters.SearchFolder, searchParameters.SearchPattern, enumerationOptions);

        return fileList;
    }
}