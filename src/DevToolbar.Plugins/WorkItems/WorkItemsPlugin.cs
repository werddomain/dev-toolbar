namespace DevToolbar.Plugins.WorkItems;

using DevToolbar.Core.Interfaces;
using DevToolbar.Core.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

/// <summary>
/// Plugin for displaying and managing work items (TFS/GitHub Issues).
/// </summary>
public class WorkItemsPlugin : IPlugin
{
    private readonly IWorkItemProvider _provider;

    public string UniqueId => "work-items";
    public string Name => "Work Items";
    public string Icon => "📋";
    public bool IsEnabled { get; set; } = true;

    private WorkItem? _activeItem;
    private IReadOnlyList<WorkItem> _recentItems = Array.Empty<WorkItem>();

    public WorkItemsPlugin(IWorkItemProvider provider)
    {
        _provider = provider;
    }

    public async Task OnProjectChangedAsync(PluginContext context)
    {
        // Load recent work items for the project
        _recentItems = await _provider.SearchAsync(context.Project.Name);
        _activeItem = _recentItems.FirstOrDefault();
    }

    public RenderFragment? Render() => builder =>
    {
        builder.OpenElement(0, "div");
        builder.AddAttribute(1, "class", "plugin-workitems");

        // Active work item display
        builder.OpenElement(2, "div");
        builder.AddAttribute(3, "class", "workitem-active");

        if (_activeItem != null)
        {
            builder.OpenElement(4, "span");
            builder.AddAttribute(5, "class", "workitem-id");
            builder.AddContent(6, $"#{_activeItem.Id}");
            builder.CloseElement();

            builder.OpenElement(7, "span");
            builder.AddAttribute(8, "class", "workitem-title");
            builder.AddContent(9, _activeItem.Title);
            builder.CloseElement();

            builder.OpenElement(10, "span");
            builder.AddAttribute(11, "class", $"workitem-state workitem-state-{_activeItem.State.ToLowerInvariant().Replace(" ", "-")}");
            builder.AddContent(12, _activeItem.State);
            builder.CloseElement();
        }
        else
        {
            builder.OpenElement(4, "span");
            builder.AddAttribute(5, "class", "workitem-none");
            builder.AddContent(6, "No active work item");
            builder.CloseElement();
        }

        builder.CloseElement(); // workitem-active

        // Recent items list
        if (_recentItems.Count > 0)
        {
            builder.OpenElement(13, "div");
            builder.AddAttribute(14, "class", "workitem-recent");

            builder.OpenElement(15, "span");
            builder.AddAttribute(16, "class", "workitem-recent-label");
            builder.AddContent(17, "Recent:");
            builder.CloseElement();

            foreach (var item in _recentItems.Take(3))
            {
                builder.OpenElement(18, "div");
                builder.AddAttribute(19, "class", "workitem-entry");

                builder.OpenElement(20, "a");
                builder.AddAttribute(21, "href", _provider.GetWebUrl(item));
                builder.AddAttribute(22, "target", "_blank");
                builder.AddAttribute(23, "class", "workitem-link");
                builder.AddContent(24, $"#{item.Id} - {item.Title}");
                builder.CloseElement();

                builder.CloseElement(); // workitem-entry
            }

            builder.CloseElement(); // workitem-recent
        }

        builder.CloseElement(); // plugin-workitems
    };
}
