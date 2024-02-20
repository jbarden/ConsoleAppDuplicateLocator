namespace TestProject1.MoveToRealProject;

internal record SearchParameters
{
    public SearchParameters(string searchFolder, bool recursiveSubDirectories, string searchPattern)
    {
        SearchFolder = searchFolder;
        RecursiveSubDirectories = recursiveSubDirectories;
        SearchPattern = searchPattern;
    }

    public string SearchFolder { get; }

    public string SearchPattern { get; }

    public bool RecursiveSubDirectories { get; }
}