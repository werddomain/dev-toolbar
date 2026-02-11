namespace DevToolbar.Core.Interfaces;

using DevToolbar.Core.Models;
using Microsoft.AspNetCore.Components;

/// <summary>
/// Interface that all toolbar plugins must implement.
/// </summary>
public interface IPlugin
{
    /// <summary>Unique identifier for the plugin.</summary>
    string UniqueId { get; }

    /// <summary>Display name of the plugin.</summary>
    string Name { get; }

    /// <summary>Icon CSS class (e.g. a Material Icon name).</summary>
    string Icon { get; }

    /// <summary>Whether the plugin is currently enabled.</summary>
    bool IsEnabled { get; set; }

    /// <summary>Called when the active project changes.</summary>
    Task OnProjectChangedAsync(PluginContext context);

    /// <summary>Returns a RenderFragment to display in the plugin zone.</summary>
    RenderFragment? Render();
}
