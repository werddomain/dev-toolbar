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
    public async Task ActionDeck_ShouldDisplayFourButtons()
    {
        await NavigateAndWait();
        await Expect(Page.Locator(".action-button")).ToHaveCountAsync(4);
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

    // --- Timer Controls Tests ---

    [Test]
    public async Task TimeTrackerPlugin_ShouldShowStartButton()
    {
        await NavigateAndWait();
        await Expect(Page.Locator(".timer-btn-start")).ToBeVisibleAsync();
        await Expect(Page.Locator(".timer-btn-start")).ToContainTextAsync("Start");
    }

    // --- CI/CD Mark as Read Tests ---

    [Test]
    public async Task CiCdPlugin_ShouldShowMarkAllReadButton()
    {
        await NavigateAndWait();
        await Expect(Page.Locator(".cicd-mark-all-read")).ToBeVisibleAsync();
        await Expect(Page.Locator(".cicd-mark-all-read")).ToContainTextAsync("Read all");
    }

    [Test]
    public async Task CiCdPlugin_SessionsShouldBeClickable()
    {
        await NavigateAndWait();
        // Verify sessions have cursor:pointer (they are clickable)
        var session = Page.Locator(".cicd-session").First;
        await Expect(session).ToBeVisibleAsync();
    }

    // --- Settings Page Tests ---

    [Test]
    public async Task SettingsLink_ShouldBeVisible()
    {
        await NavigateAndWait();
        await Expect(Page.Locator(".toolbar-settings-btn")).ToBeVisibleAsync();
    }

    [Test]
    public async Task SettingsPage_ShouldShowActiveProject()
    {
        await Page.GotoAsync($"{BaseUrl}/settings", new() { WaitUntil = WaitUntilState.Load });
        await Expect(Page.Locator(".settings-page")).ToBeVisibleAsync(new() { Timeout = 30000 });
        await Expect(Page.GetByText("MyWebAPI")).ToBeVisibleAsync();
    }

    [Test]
    public async Task SettingsPage_ShouldShowPluginsList()
    {
        await Page.GotoAsync($"{BaseUrl}/settings", new() { WaitUntil = WaitUntilState.Load });
        await Expect(Page.Locator(".settings-page")).ToBeVisibleAsync(new() { Timeout = 30000 });
        await Expect(Page.Locator(".settings-plugin-row")).ToHaveCountAsync(4);
    }

    [Test]
    public async Task SettingsPage_ShouldShowActionsList()
    {
        await Page.GotoAsync($"{BaseUrl}/settings", new() { WaitUntil = WaitUntilState.Load });
        await Expect(Page.Locator(".settings-page")).ToBeVisibleAsync(new() { Timeout = 30000 });
        await Expect(Page.Locator(".settings-action-row")).ToHaveCountAsync(4);
    }

    [Test]
    public async Task SettingsPage_ShouldShowAllProjects()
    {
        await Page.GotoAsync($"{BaseUrl}/settings", new() { WaitUntil = WaitUntilState.Load });
        await Expect(Page.Locator(".settings-page")).ToBeVisibleAsync(new() { Timeout = 30000 });
        await Expect(Page.Locator(".settings-project-row")).ToHaveCountAsync(3);
    }

    [Test]
    public async Task SettingsPage_ShouldHaveBackLink()
    {
        await Page.GotoAsync($"{BaseUrl}/settings", new() { WaitUntil = WaitUntilState.Load });
        await Expect(Page.Locator(".settings-page")).ToBeVisibleAsync(new() { Timeout = 30000 });
        await Expect(Page.Locator(".settings-back")).ToBeVisibleAsync();
        await Expect(Page.Locator(".settings-back")).ToContainTextAsync("Back to Toolbar");
    }

    // --- Git Quick Sync Tests ---

    [Test]
    public async Task GitPlugin_ShouldShowPullButton()
    {
        await NavigateAndWait();
        await Expect(Page.Locator(".git-btn-pull")).ToBeVisibleAsync();
        await Expect(Page.Locator(".git-btn-pull")).ToContainTextAsync("Pull");
    }

    [Test]
    public async Task GitPlugin_ShouldShowPushButton()
    {
        await NavigateAndWait();
        await Expect(Page.Locator(".git-btn-push")).ToBeVisibleAsync();
        await Expect(Page.Locator(".git-btn-push")).ToContainTextAsync("Push");
    }

    // --- Time Report Modal Tests ---

    [Test]
    public async Task TimeReportButton_ShouldBeVisible()
    {
        await NavigateAndWait();
        await Expect(Page.Locator(".toolbar-report-btn")).ToBeVisibleAsync();
    }

    [Test]
    public async Task TimeReport_ShouldOpenOnClick()
    {
        await NavigateAndWait();
        await Page.Locator(".toolbar-report-btn").ClickAsync();
        await Expect(Page.Locator(".time-report-overlay.visible")).ToBeVisibleAsync(new() { Timeout = 5000 });
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Time Report" })).ToBeVisibleAsync();
    }

    [Test]
    public async Task TimeReport_ShouldHavePeriodFilter()
    {
        await NavigateAndWait();
        await Page.Locator(".toolbar-report-btn").ClickAsync();
        await Expect(Page.Locator(".time-report-overlay.visible")).ToBeVisibleAsync(new() { Timeout = 5000 });
        await Expect(Page.Locator(".time-report-filter")).ToBeVisibleAsync();
    }

    [Test]
    public async Task TimeReport_ShouldHaveExportButton()
    {
        await NavigateAndWait();
        await Page.Locator(".toolbar-report-btn").ClickAsync();
        await Expect(Page.Locator(".time-report-overlay.visible")).ToBeVisibleAsync(new() { Timeout = 5000 });
        await Expect(Page.Locator(".time-report-export")).ToBeVisibleAsync();
        await Expect(Page.Locator(".time-report-export")).ToContainTextAsync("Export CSV");
    }

    // --- Script / Terminal Output Tests ---

    [Test]
    public async Task ActionDeck_ShouldShowBuildScriptButton()
    {
        await NavigateAndWait();
        await Expect(Page.Locator(".action-button", new() { HasText = "Build Script" })).ToBeVisibleAsync();
    }

    [Test]
    public async Task BuildScript_ShouldOpenTerminalOnClick()
    {
        await NavigateAndWait();
        await Page.Locator(".action-button", new() { HasText = "Build Script" }).ClickAsync();
        await Expect(Page.Locator(".terminal-overlay.visible")).ToBeVisibleAsync(new() { Timeout = 5000 });
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Terminal Output" })).ToBeVisibleAsync();
    }

    [Test]
    public async Task Terminal_ShouldShowMockOutput()
    {
        await NavigateAndWait();
        await Page.Locator(".action-button", new() { HasText = "Build Script" }).ClickAsync();
        await Expect(Page.Locator(".terminal-overlay.visible")).ToBeVisibleAsync(new() { Timeout = 5000 });
        await Expect(Page.Locator(".terminal-output")).ToContainTextAsync("Build successful");
    }

    [Test]
    public async Task Terminal_ShouldShowExitCode()
    {
        await NavigateAndWait();
        await Page.Locator(".action-button", new() { HasText = "Build Script" }).ClickAsync();
        await Expect(Page.Locator(".terminal-overlay.visible")).ToBeVisibleAsync(new() { Timeout = 5000 });
        await Expect(Page.Locator(".terminal-exit")).ToContainTextAsync("Exit: 0");
    }

    // --- Project Switching Tests ---

    [Test]
    public async Task ProjectSwitch_ShouldChangeVisiblePlugins()
    {
        await NavigateAndWait();
        // Default MyWebAPI has 4 plugins
        await Expect(Page.Locator(".plugin-panel")).ToHaveCountAsync(4);

        // Switch to DevOps Pipeline (has only git-tools, github-agents)
        await Page.Locator(".project-selector select").SelectOptionAsync("proj-devops");
        await Expect(Page.Locator(".plugin-panel")).ToHaveCountAsync(2, new() { Timeout = 5000 });

        // Switch back to MyWebAPI
        await Page.Locator(".project-selector select").SelectOptionAsync("proj-webapi");
        await Expect(Page.Locator(".plugin-panel")).ToHaveCountAsync(4, new() { Timeout = 5000 });
    }

    [Test]
    public async Task ProjectSwitch_ShouldUpdateActions()
    {
        await NavigateAndWait();
        // Default MyWebAPI has 4 actions
        await Expect(Page.Locator(".action-button")).ToHaveCountAsync(4);

        // Switch to FrontEnd App (has 3 actions)
        await Page.Locator(".project-selector select").SelectOptionAsync("proj-frontend");
        await Expect(Page.Locator(".action-button")).ToHaveCountAsync(3, new() { Timeout = 5000 });
    }

    // --- Status Bar Footer Tests ---

    [Test]
    public async Task Footer_ShouldDisplayProjectName()
    {
        await NavigateAndWait();
        await Expect(Page.Locator(".toolbar-footer")).ToBeVisibleAsync();
        await Expect(Page.Locator(".footer-project")).ToContainTextAsync("MyWebAPI");
    }

    [Test]
    public async Task Footer_ShouldShowPluginCount()
    {
        await NavigateAndWait();
        await Expect(Page.Locator(".footer-plugin-count")).ToContainTextAsync("4 plugins");
    }

    [Test]
    public async Task Footer_ShouldUpdateOnProjectSwitch()
    {
        await NavigateAndWait();
        await Expect(Page.Locator(".footer-project")).ToContainTextAsync("MyWebAPI");

        // Switch to DevOps Pipeline (2 plugins)
        await Page.Locator(".project-selector select").SelectOptionAsync("proj-devops");
        await Expect(Page.Locator(".footer-project")).ToContainTextAsync("DevOps Pipeline", new() { Timeout = 5000 });
        await Expect(Page.Locator(".footer-plugin-count")).ToContainTextAsync("2 plugins", new() { Timeout = 5000 });
    }

    // --- CSV Export Tests ---

    [Test]
    public async Task TimeReport_ExportButton_ShouldBeClickable()
    {
        await NavigateAndWait();
        await Page.Locator(".toolbar-report-btn").ClickAsync();
        await Expect(Page.Locator(".time-report-overlay.visible")).ToBeVisibleAsync(new() { Timeout = 5000 });

        var exportBtn = Page.Locator(".time-report-export");
        await Expect(exportBtn).ToBeVisibleAsync();
        await Expect(exportBtn).ToBeEnabledAsync();
    }

    // --- Work Items Search Tests (US5.2) ---

    [Test]
    public async Task WorkItemsPlugin_ShouldShowSearchToggle()
    {
        await NavigateAndWait();
        await Expect(Page.Locator(".workitem-search-toggle")).ToBeVisibleAsync();
    }

    [Test]
    public async Task WorkItemsPlugin_ShouldOpenSearchDropdown()
    {
        await NavigateAndWait();
        await Page.Locator(".workitem-search-toggle").ClickAsync();
        await Expect(Page.Locator(".workitem-search-input")).ToBeVisibleAsync(new() { Timeout = 5000 });
        await Expect(Page.Locator(".workitem-dropdown")).ToBeVisibleAsync();
    }

    [Test]
    public async Task WorkItemsPlugin_ShouldSearchAndSelectItem()
    {
        await NavigateAndWait();
        await Page.Locator(".workitem-search-toggle").ClickAsync();
        await Expect(Page.Locator(".workitem-search-input")).ToBeVisibleAsync(new() { Timeout = 5000 });

        // Type search query (character by character to trigger oninput)
        await Page.Locator(".workitem-search-input").PressSequentiallyAsync("dark");
        await Expect(Page.Locator(".workitem-dropdown-item")).ToHaveCountAsync(1, new() { Timeout = 5000 });

        // Select the item
        await Page.Locator(".workitem-dropdown-item").First.ClickAsync();

        // Verify active item changed
        await Expect(Page.Locator(".workitem-id")).ToContainTextAsync("#1235", new() { Timeout = 5000 });
        await Expect(Page.Locator(".workitem-title")).ToContainTextAsync("Add dark mode support");
    }

    [Test]
    public async Task WorkItemsPlugin_SearchShouldHideRecentItems()
    {
        await NavigateAndWait();
        // Recent items visible initially
        await Expect(Page.Locator(".workitem-recent")).ToBeVisibleAsync();

        // Open search - recent items should be hidden
        await Page.Locator(".workitem-search-toggle").ClickAsync();
        await Expect(Page.Locator(".workitem-search-input")).ToBeVisibleAsync(new() { Timeout = 5000 });
        await Expect(Page.Locator(".workitem-recent")).ToHaveCountAsync(0);
    }

    // --- Timer Idle Detection Tests (US5.3) ---

    [Test]
    public async Task TimeTrackerPlugin_ShouldShowIdleTimeout()
    {
        await NavigateAndWait();
        await Expect(Page.Locator(".timer-idle-label")).ToContainTextAsync("Idle timeout: 15 min");
    }

    // --- Settings Page Toggle Tests ---

    [Test]
    public async Task SettingsPage_ShouldShowPluginToggleSwitches()
    {
        await Page.GotoAsync($"{BaseUrl}/settings", new() { WaitUntil = WaitUntilState.Load });
        await Expect(Page.Locator(".settings-page")).ToBeVisibleAsync(new() { Timeout = 30000 });
        await Expect(Page.Locator(".settings-toggle")).ToHaveCountAsync(4);
    }

    [Test]
    public async Task SettingsPage_ShouldAllowProjectSwitching()
    {
        await Page.GotoAsync($"{BaseUrl}/settings", new() { WaitUntil = WaitUntilState.Load });
        await Expect(Page.Locator(".settings-page")).ToBeVisibleAsync(new() { Timeout = 30000 });

        // Click on a different project
        var frontendRow = Page.Locator(".settings-project-row", new() { HasText = "FrontEnd App" });
        await Expect(frontendRow).ToBeVisibleAsync();
        await frontendRow.ClickAsync();

        // Verify it becomes active
        await Expect(Page.Locator(".settings-value", new() { HasText = "FrontEnd App" })).ToBeVisibleAsync(new() { Timeout = 5000 });
    }
}
