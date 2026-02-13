namespace DevToolbar.Web.Mocks;

using DevToolbar.Core.Interfaces;

/// <summary>
/// Mock script service for web testing.
/// </summary>
public class MockScriptService : IScriptService
{
    public Task<ScriptResult> ExecuteAsync(string interpreter, string scriptPath, string arguments = "")
    {
        Console.WriteLine($"[MockScript] {interpreter} {scriptPath} {arguments}");
        return Task.FromResult(new ScriptResult
        {
            ExitCode = 0,
            Output = $"[Mock] Executed: {interpreter} {scriptPath} {arguments}\nBuild successful.\n3 tests passed.",
            Error = string.Empty
        });
    }
}
