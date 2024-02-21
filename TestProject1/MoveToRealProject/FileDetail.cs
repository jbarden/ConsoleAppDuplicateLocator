
namespace TestProject1.MoveToRealProject;

public class FileDetail
{
    public FileDetail(FileInfo fileInfo)
    {
        FullName = fileInfo.FullName;
        FileName = fileInfo.Name;
        FileSize = fileInfo.Length;
        Extension = fileInfo.Extension;
        Width = 0;
        Height = 0;
    }

    public string FullName { get; }
    public string FileName { get; }
    public long FileSize { get; }
    public string Extension { get; }
    public int Width { get; }
    public int Height { get; }
}