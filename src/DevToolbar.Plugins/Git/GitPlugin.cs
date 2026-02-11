namespace DevToolbar.Plugins.Git;

using DevToolbar.Core.Interfaces;
using DevToolbar.Core.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

/// <summary>
/// Plugin showing Git branch status for the active project.
/// </summary>
public class GitPlugin : IPlugin
{
    public string UniqueId => "git-tools";
    public string Name => "Git Tools";
    public string Icon => "git";
    public bool IsEnabled { get; set; } = true;

    private string _currentBranch = "main";
    private bool _isDirty = false;

    public Task OnProjectChangedAsync(PluginContext context)
    {
        // In a real implementation, use LibGit2Sharp or git CLI
        _currentBranch = "main";
        _isDirty = false;
        return Task.CompletedTask;
    }

    public RenderFragment? Render() => builder =>
    {
        builder.OpenElement(0, "div");
        builder.AddAttribute(1, "class", "plugin-git");

        builder.OpenElement(2, "span");
        builder.AddAttribute(3, "class", "git-branch");
        builder.AddContent(4, $"⎇ {_currentBranch}");
        builder.CloseElement();

        builder.OpenElement(5, "span");
        builder.AddAttribute(6, "class", _isDirty ? "git-status dirty" : "git-status clean");
        builder.AddContent(7, _isDirty ? "● Modified" : "✓ Clean");
        builder.CloseElement();

        builder.CloseElement();
    };
}
