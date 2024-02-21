using System.IO.Abstractions;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using TestProject1.MoveToRealProject;
using Xunit.Abstractions;

namespace TestProject1;

public class DuplicateFileFinderShould
{
    private ITestOutputHelper OutputHelper { get; }
    private string TestFilesPath { get; }

    public DuplicateFileFinderShould(ITestOutputHelper outputHelper)
    {
        OutputHelper = outputHelper;
        var runningPath = Assembly.GetExecutingAssembly().Location;
        TestFilesPath = Path.Combine(runningPath, "..", "..", "..", "..", "TestFiles");
        OutputHelper.WriteLine($"TestFilesPath: {TestFilesPath}");
    }

    [Fact]
    public void ReturnTheExpectedNumberOfFilesWhenNotFilteredAtAll()
    {
        const int ActualNumberOfTestFiles = 20;

        var duplicateFileList = new DuplicateFileFinder(new FileSystem()).GetInitialFilesMatchingSearchCriteria(new SearchParameters(TestFilesPath, true, "*.*"));

        duplicateFileList.Count().Should().Be(ActualNumberOfTestFiles);
    }

    [Fact]
    public Task ReturnTheExpectedListOfFilesWhenNotFilteredAtAll()
    {
        var duplicateFileList = new DuplicateFileFinder(new FileSystem()).GetInitialFilesMatchingSearchCriteria(new SearchParameters(TestFilesPath, true, "*.*"))
            .GetFileSizes();
        JsonSerializerOptions options = new(JsonSerializerDefaults.Web);

        var first = JsonSerializer.Serialize(duplicateFileList.First());
        var all = JsonSerializer.Serialize(duplicateFileList, options);

        OutputHelper.WriteLine(first);
        OutputHelper.WriteLine(all);
        return VerifyJson(JsonSerializer.Serialize(duplicateFileList, options ));
    }
}