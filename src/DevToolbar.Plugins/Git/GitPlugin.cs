namespace DevToolbar.Plugins.Git;

using DevToolbar.Core.Interfaces;
using DevToolbar.Core.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

/// <summary>
/// Plugin showing Git branch status with Quick Sync (Pull/Push) for the active project.
/// </summary>
public class GitPlugin : IPlugin
{
    private readonly IProcessService _processService;

    public string UniqueId => "git-tools";
    public string Name => "Git Tools";
    public string Icon => "git";
    public bool IsEnabled { get; set; } = true;
    public event Action? OnStateChanged;

    private string _currentBranch = "main";
    private bool _isDirty = false;
    private string _syncStatus = string.Empty;

    public GitPlugin(IProcessService processService)
    {
        _processService = processService;
    }

    public Task OnProjectChangedAsync(PluginContext context)
    {
        _currentBranch = "main";
        _isDirty = false;
        _syncStatus = string.Empty;
        return Task.CompletedTask;
    }

    public RenderFragment? Render() => builder =>
    {
        builder.OpenElement(0, "div");
        builder.AddAttribute(1, "class", "plugin-git");

        // Branch info row
        builder.OpenElement(2, "div");
        builder.AddAttribute(3, "class", "git-info");

        builder.OpenElement(4, "span");
        builder.AddAttribute(5, "class", "git-branch");
        builder.AddContent(6, $"⎇ {_currentBranch}");
        builder.CloseElement();

        builder.OpenElement(7, "span");
        builder.AddAttribute(8, "class", _isDirty ? "git-status dirty" : "git-status clean");
        builder.AddContent(9, _isDirty ? "● Modified" : "✓ Clean");
        builder.CloseElement();

        builder.CloseElement(); // git-info

        // Quick Sync buttons row
        builder.OpenElement(10, "div");
        builder.AddAttribute(11, "class", "git-sync");

        builder.OpenElement(12, "button");
        builder.AddAttribute(13, "class", "git-btn git-btn-pull");
        builder.AddAttribute(14, "title", "Pull latest changes");
        builder.AddAttribute(15, "onclick", EventCallback.Factory.Create(this, async () =>
        {
            try
            {
                _syncStatus = "Pulling...";
                OnStateChanged?.Invoke();
                await _processService.StartProcessAsync("git", "pull");
                _syncStatus = "✓ Pull complete";
            }
            catch (Exception ex)
            {
                _syncStatus = $"✗ Pull failed: {ex.Message}";
            }
            OnStateChanged?.Invoke();
        }));
        builder.AddContent(16, "⬇ Pull");
        builder.CloseElement();

        builder.OpenElement(17, "button");
        builder.AddAttribute(18, "class", "git-btn git-btn-push");
        builder.AddAttribute(19, "title", "Push local changes");
        builder.AddAttribute(20, "onclick", EventCallback.Factory.Create(this, async () =>
        {
            try
            {
                _syncStatus = "Pushing...";
                OnStateChanged?.Invoke();
                await _processService.StartProcessAsync("git", "push");
                _syncStatus = "✓ Push complete";
            }
            catch (Exception ex)
            {
                _syncStatus = $"✗ Push failed: {ex.Message}";
            }
            OnStateChanged?.Invoke();
        }));
        builder.AddContent(21, "⬆ Push");
        builder.CloseElement();

        builder.CloseElement(); // git-sync

        // Sync status message
        if (!string.IsNullOrEmpty(_syncStatus))
        {
            builder.OpenElement(22, "div");
            builder.AddAttribute(23, "class", "git-sync-status");
            builder.AddContent(24, _syncStatus);
            builder.CloseElement();
        }

        builder.CloseElement(); // plugin-git
    };
}
