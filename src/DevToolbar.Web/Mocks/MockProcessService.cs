namespace DevToolbar.Web.Mocks;

using DevToolbar.Core.Interfaces;

/// <summary>
/// Mock process service for web testing. Logs actions without executing them.
/// </summary>
public class MockProcessService : IProcessService
{
    private readonly ILogger<MockProcessService> _logger;

    public MockProcessService(ILogger<MockProcessService> logger)
    {
        _logger = logger;
    }

    public Task<int> StartProcessAsync(string path, string arguments = "")
    {
        _logger.LogInformation("[Mock] StartProcess: {Path} {Arguments}", path, arguments);
        return Task.FromResult(0);
    }

    public bool FocusWindowByTitle(string titleRegex)
    {
        _logger.LogInformation("[Mock] FocusWindow: {TitleRegex}", titleRegex);
        return false;
    }
}
