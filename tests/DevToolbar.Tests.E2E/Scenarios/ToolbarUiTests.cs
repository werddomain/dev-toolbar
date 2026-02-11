namespace DevToolbar.Tests.E2E.Scenarios;

using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

/// <summary>
/// E2E tests for the DevToolbar UI using Playwright.
/// Tests run against the DevToolbar.Web project hosted on localhost.
/// </summary>
[TestFixture]
public class ToolbarUiTests : PageTest
{
    private const string BaseUrl = "http://localhost:5280";
    private System.Diagnostics.Process? _serverProcess;

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        // Start the web server
        var projectPath = Path.GetFullPath(
            Path.Combine(TestContext.CurrentContext.TestDirectory, "..", "..", "..", "..", "..",
                "src", "DevToolbar.Web"));

        _serverProcess = new System.Diagnostics.Process
        {
            StartInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"run --project \"{projectPath}\" --urls {BaseUrl}",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                Environment = { ["ASPNETCORE_ENVIRONMENT"] = "Development" }
            }
        };

        _serverProcess.Start();

        // Wait for server to be ready
        using var httpClient = new HttpClient();
        var maxRetries = 30;
        for (var i = 0; i < maxRetries; i++)
        {
            try
            {
                var response = await httpClient.GetAsync(BaseUrl);
                if (response.IsSuccessStatusCode)
                    break;
            }
            catch
            {
                // Server not ready yet
            }
            await Task.Delay(1000);
        }
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        if (_serverProcess != null && !_serverProcess.HasExited)
        {
            _serverProcess.Kill(entireProcessTree: true);
            _serverProcess.Dispose();
        }
    }

    [Test]
    public async Task PageTitle_ShouldBeDevToolbar()
    {
        await Page.GotoAsync(BaseUrl);
        await Expect(Page).ToHaveTitleAsync("DevToolbar");
    }

    [Test]
    public async Task Header_ShouldDisplayBrandName()
    {
        await Page.GotoAsync(BaseUrl);
        var brandTitle = Page.Locator(".brand-title");
        await Expect(brandTitle).ToHaveTextAsync("DevToolbar");
    }

    [Test]
    public async Task ProjectSelector_ShouldHaveThreeOptions()
    {
        await Page.GotoAsync(BaseUrl);
        var select = Page.Locator(".project-selector select");
        var options = select.Locator("option");
        await Expect(options).ToHaveCountAsync(3);
    }

    [Test]
    public async Task ProjectSelector_DefaultShouldBeMyWebAPI()
    {
        await Page.GotoAsync(BaseUrl);
        var select = Page.Locator(".project-selector select");
        await Expect(select).ToHaveValueAsync("proj-webapi");
    }

    [Test]
    public async Task GitPlugin_ShouldShowBranchAndStatus()
    {
        await Page.GotoAsync(BaseUrl);
        var gitBranch = Page.Locator(".git-branch");
        await Expect(gitBranch).ToContainTextAsync("main");

        var gitStatus = Page.Locator(".git-status");
        await Expect(gitStatus).ToContainTextAsync("Clean");
    }

    [Test]
    public async Task ActionDeck_ShouldDisplayButtons()
    {
        await Page.GotoAsync(BaseUrl);
        var buttons = Page.Locator(".action-button");
        await Expect(buttons).ToHaveCountAsync(3);
    }

    [Test]
    public async Task ActionDeck_ShouldShowVisualStudioButton()
    {
        await Page.GotoAsync(BaseUrl);
        var vsButton = Page.Locator(".action-button", new() { HasText = "Visual Studio" });
        await Expect(vsButton).ToBeVisibleAsync();
    }
}
