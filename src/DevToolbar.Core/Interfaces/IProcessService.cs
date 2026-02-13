namespace DevToolbar.Core.Interfaces;

/// <summary>
/// Abstracts process and window management operations.
/// </summary>
public interface IProcessService
{
    /// <summary>Start a process and return its PID.</summary>
    Task<int> StartProcessAsync(string path, string arguments = "");

    /// <summary>Focus an existing window by title regex. Returns true if found.</summary>
    bool FocusWindowByTitle(string titleRegex);
}
