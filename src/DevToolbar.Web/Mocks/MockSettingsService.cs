namespace DevToolbar.Web.Mocks;

using DevToolbar.Core.Interfaces;
using DevToolbar.Core.Models;

/// <summary>
/// Mock settings service with sample project data for web testing.
/// </summary>
public class MockSettingsService : ISettingsService
{
    private readonly List<ProjectConfig> _projects;
    private ProjectConfig? _activeProject;

    public MockSettingsService()
    {
        _projects = new List<ProjectConfig>
        {
            new ProjectConfig
            {
                Id = "proj-webapi",
                Name = "MyWebAPI",
                Path = "/projects/my-webapi",
                ProjectType = "WebApi",
                Theme = new ThemeConfig { AccentColor = "#0078D7" },
                EnabledPlugins = new List<string> { "git-tools" },
                Actions = new List<ActionConfig>
                {
                    new ActionConfig { Label = "Visual Studio", Icon = "🟣", ProcessPath = "devenv.exe" },
                    new ActionConfig { Label = "Postman", Icon = "🟠", ProcessPath = "postman.exe" },
                    new ActionConfig { Label = "Terminal", Icon = "⬛", ProcessPath = "wt.exe" }
                }
            },
            new ProjectConfig
            {
                Id = "proj-frontend",
                Name = "FrontEnd App",
                Path = "/projects/frontend",
                ProjectType = "SPA",
                Theme = new ThemeConfig { AccentColor = "#4CAF50" },
                EnabledPlugins = new List<string> { "git-tools" },
                Actions = new List<ActionConfig>
                {
                    new ActionConfig { Label = "VS Code", Icon = "🔵", ProcessPath = "code.exe" },
                    new ActionConfig { Label = "Browser", Icon = "🌐", ProcessPath = "chrome.exe" }
                }
            },
            new ProjectConfig
            {
                Id = "proj-devops",
                Name = "DevOps Pipeline",
                Path = "/projects/devops",
                ProjectType = "Infrastructure",
                Theme = new ThemeConfig { AccentColor = "#E53935" },
                EnabledPlugins = new List<string> { "git-tools" },
                Actions = new List<ActionConfig>
                {
                    new ActionConfig { Label = "Azure Portal", Icon = "☁️", ProcessPath = "https://portal.azure.com", WindowTitleRegex = ".*Azure.*" }
                }
            }
        };

        _activeProject = _projects[0];
    }

    public Task<IReadOnlyList<ProjectConfig>> GetProjectsAsync() =>
        Task.FromResult<IReadOnlyList<ProjectConfig>>(_projects.AsReadOnly());

    public ProjectConfig? GetActiveProject() => _activeProject;

    public Task SetActiveProjectAsync(string projectId)
    {
        _activeProject = _projects.FirstOrDefault(p => p.Id == projectId);
        return Task.CompletedTask;
    }
}
