namespace DevToolbar.Core.Interfaces;

using DevToolbar.Core.Models;

/// <summary>
/// Manages hierarchical configuration (global, template, project-local).
/// </summary>
public interface ISettingsService
{
    /// <summary>Get the list of configured projects.</summary>
    Task<IReadOnlyList<ProjectConfig>> GetProjectsAsync();

    /// <summary>Get the currently active project.</summary>
    ProjectConfig? GetActiveProject();

    /// <summary>Set the active project by ID.</summary>
    Task SetActiveProjectAsync(string projectId);

    /// <summary>Save changes to a project configuration (US8.3).</summary>
    Task SaveProjectAsync(ProjectConfig project);

    /// <summary>Event fired when the active project changes.</summary>
    event Action<ProjectConfig>? OnActiveProjectChanged;
}
