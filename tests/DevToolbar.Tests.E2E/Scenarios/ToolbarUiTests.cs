namespace DevToolbar.Tests.E2E.Scenarios;

using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

/// <summary>
/// E2E tests for the DevToolbar UI using Playwright.
/// Tests run against the DevToolbar.Web project hosted on localhost.
/// </summary>
[TestFixture]
[Parallelizable(ParallelScope.None)]
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
        var maxRetries = 60;
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

    /// <summary>
    /// Navigate to the app and wait for SSR content to appear.
    /// </summary>
    private async Task NavigateAndWait()
    {
        await Page.GotoAsync(BaseUrl, new() { WaitUntil = WaitUntilState.Load });
        // The SSR-rendered content includes .toolbar-shell immediately
        await Expect(Page.Locator(".toolbar-shell")).ToBeVisibleAsync(new() { Timeout = 30000 });
    }

    [Test]
    public async Task PageTitle_ShouldBeDevToolbar()
    {
        await Page.GotoAsync(BaseUrl, new() { WaitUntil = WaitUntilState.Load });
        await Expect(Page).ToHaveTitleAsync("DevToolbar", new() { Timeout = 30000 });
    }

    [Test]
    public async Task Header_ShouldDisplayBrandName()
    {
        await NavigateAndWait();
        await Expect(Page.Locator(".brand-title")).ToHaveTextAsync("DevToolbar");
    }

    [Test]
    public async Task ProjectSelector_ShouldHaveThreeOptions()
    {
        await NavigateAndWait();
        await Expect(Page.Locator(".project-selector select option")).ToHaveCountAsync(3);
    }

    [Test]
    public async Task ProjectSelector_DefaultShouldBeMyWebAPI()
    {
        await NavigateAndWait();
        await Expect(Page.Locator(".project-selector select")).ToHaveValueAsync("proj-webapi");
    }

    [Test]
    public async Task GitPlugin_ShouldShowBranchAndStatus()
    {
        await NavigateAndWait();
        await Expect(Page.Locator(".git-branch")).ToContainTextAsync("main");
        await Expect(Page.Locator(".git-status")).ToContainTextAsync("Clean");
    }

    [Test]
    public async Task ActionDeck_ShouldDisplayThreeButtons()
    {
        await NavigateAndWait();
        await Expect(Page.Locator(".action-button")).ToHaveCountAsync(3);
    }

    [Test]
    public async Task ActionDeck_ShouldShowVisualStudioButton()
    {
        await NavigateAndWait();
        await Expect(Page.Locator(".action-button", new() { HasText = "Visual Studio" })).ToBeVisibleAsync();
    }

    [Test]
    public async Task WorkItemsPlugin_ShouldShowActiveItem()
    {
        await NavigateAndWait();
        await Expect(Page.Locator(".workitem-id")).ToContainTextAsync("#1234");
        await Expect(Page.Locator(".workitem-title")).ToContainTextAsync("Fix login page redirect");
    }

    [Test]
    public async Task WorkItemsPlugin_ShouldShowRecentItems()
    {
        await NavigateAndWait();
        await Expect(Page.Locator(".workitem-link")).ToHaveCountAsync(3);
    }

    [Test]
    public async Task TimeTrackerPlugin_ShouldShowStoppedState()
    {
        await NavigateAndWait();
        await Expect(Page.Locator(".timer-status")).ToContainTextAsync("Stopped");
    }

    [Test]
    public async Task TimeTrackerPlugin_ShouldShowTodayTotal()
    {
        await NavigateAndWait();
        await Expect(Page.Locator(".timer-today-label")).ToContainTextAsync("Today:");
    }

    [Test]
    public async Task CiCdPlugin_ShouldShowPipelinesLabel()
    {
        await NavigateAndWait();
        await Expect(Page.Locator(".cicd-label")).ToContainTextAsync("Pipelines");
    }

    [Test]
    public async Task CiCdPlugin_ShouldShowUnreadBadge()
    {
        await NavigateAndWait();
        await Expect(Page.Locator(".cicd-badge")).ToBeVisibleAsync();
        await Expect(Page.Locator(".cicd-badge")).ToContainTextAsync("2");
    }

    [Test]
    public async Task CiCdPlugin_ShouldShowSessions()
    {
        await NavigateAndWait();
        await Expect(Page.Locator(".cicd-session")).ToHaveCountAsync(3);
    }

    [Test]
    public async Task MyWebAPI_ShouldShowAllFourPlugins()
    {
        await NavigateAndWait();
        await Expect(Page.Locator(".plugin-panel")).ToHaveCountAsync(4);
    }
}
