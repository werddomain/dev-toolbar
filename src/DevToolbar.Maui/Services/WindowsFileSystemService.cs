using DevToolbar.Core.Interfaces;

namespace DevToolbar.Maui.Services;

/// <summary>
/// Real file system service for desktop (MAUI).
/// </summary>
public class WindowsFileSystemService : IFileSystemService
{
    public Task<string> ReadFileAsync(string path) => File.ReadAllTextAsync(path);

    public Task WriteFileAsync(string path, string content) => File.WriteAllTextAsync(path, content);

    public bool FileExists(string path) => File.Exists(path);

    public bool DirectoryExists(string path) => Directory.Exists(path);
}
