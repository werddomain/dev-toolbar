namespace DevToolbar.Core.Interfaces;

/// <summary>
/// Service for executing scripts with output streaming.
/// </summary>
public interface IScriptService
{
    /// <summary>Execute a script and return the output.</summary>
    Task<ScriptResult> ExecuteAsync(string interpreter, string scriptPath, string arguments = "");
}

/// <summary>
/// Result of a script execution.
/// </summary>
public class ScriptResult
{
    public int ExitCode { get; set; }
    public string Output { get; set; } = string.Empty;
    public string Error { get; set; } = string.Empty;
    public bool Success => ExitCode == 0;
}
