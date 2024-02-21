namespace TestProject1.MoveToRealProject;

public static class IEnumerableExtensions
{
    public static IEnumerable<FileDetail> GetFileSizes(this IEnumerable<string> files)
    {
        var fileList = new List<FileDetail>();
        foreach (var file in files)
        {
            fileList.Add(new FileDetail(new FileInfo(file)));
        }

        return fileList;
    }
}
