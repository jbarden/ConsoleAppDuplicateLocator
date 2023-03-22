namespace ConsoleAppDuplicateLocator;

internal record SearchParameters
{
    public SearchParameters(int eventRaiseCounter, string searchFolder, bool recurseSubdirectories, string searchPattern)
    {
        EventRaiseCounter = eventRaiseCounter;
        SearchFolder = searchFolder;
        RecurseSubdirectories = recurseSubdirectories;
        SearchPattern = searchPattern;
    }

    public int EventRaiseCounter { get; }

    public string SearchFolder { get; }

    public string SearchPattern { get; }

    public bool RecurseSubdirectories { get; }
}