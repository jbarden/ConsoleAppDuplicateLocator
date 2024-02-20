namespace TestProject1.MoveToRealProject;

public static class IEnumerableExtensions
{
    public static IEnumerable<FileInfo> GetFileSizes(this IEnumerable<string> files)
    {
        var fileList = new List<FileInfo>();
        foreach (var file in files)
        {
            fileList.Add(new FileInfo(file));
        }

        return fileList;
    }
}
