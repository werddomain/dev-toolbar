namespace DevToolbar.Plugins.WorkItems;

using DevToolbar.Core.Interfaces;
using DevToolbar.Core.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

/// <summary>
/// Plugin for displaying and managing work items (TFS/GitHub Issues).
/// Supports dropdown search/select to change active item (US5.2).
/// </summary>
public class WorkItemsPlugin : IPlugin
{
    private readonly IWorkItemProvider _provider;

    public string UniqueId => "work-items";
    public string Name => "Work Items";
    public string Icon => "📋";
    public bool IsEnabled { get; set; } = true;
    public event Action? OnStateChanged;

    private WorkItem? _activeItem;
    private IReadOnlyList<WorkItem> _recentItems = Array.Empty<WorkItem>();
    private IReadOnlyList<WorkItem> _searchResults = Array.Empty<WorkItem>();
    private bool _showSearch;
    private string _searchQuery = string.Empty;

    public WorkItemsPlugin(IWorkItemProvider provider)
    {
        _provider = provider;
    }

    public async Task OnProjectChangedAsync(PluginContext context)
    {
        _recentItems = await _provider.SearchAsync(context.Project.Name);
        _activeItem = _recentItems.FirstOrDefault();
        _showSearch = false;
        _searchQuery = string.Empty;
        _searchResults = Array.Empty<WorkItem>();
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

        // Toggle search button
        builder.OpenElement(30, "button");
        builder.AddAttribute(31, "class", "workitem-search-toggle");
        builder.AddAttribute(32, "title", "Search work items");
        builder.AddAttribute(33, "onclick", EventCallback.Factory.Create(this, () =>
        {
            _showSearch = !_showSearch;
            if (!_showSearch)
            {
                _searchQuery = string.Empty;
                _searchResults = Array.Empty<WorkItem>();
            }
            OnStateChanged?.Invoke();
        }));
        builder.AddContent(34, _showSearch ? "✕" : "🔍");
        builder.CloseElement();

        builder.CloseElement(); // workitem-active

        // Search dropdown (US5.2)
        if (_showSearch)
        {
            builder.OpenElement(40, "div");
            builder.AddAttribute(41, "class", "workitem-search");

            builder.OpenElement(42, "input");
            builder.AddAttribute(43, "type", "text");
            builder.AddAttribute(44, "class", "workitem-search-input");
            builder.AddAttribute(45, "placeholder", "Search work items...");
            builder.AddAttribute(46, "value", _searchQuery);
            builder.AddAttribute(47, "oninput", EventCallback.Factory.Create<ChangeEventArgs>(this, async e =>
            {
                _searchQuery = e.Value?.ToString() ?? string.Empty;
                if (_searchQuery.Length >= 1)
                {
                    _searchResults = await _provider.SearchAsync(_searchQuery);
                }
                else
                {
                    _searchResults = _recentItems;
                }
                OnStateChanged?.Invoke();
            }));
            builder.CloseElement(); // input

            // Search results dropdown
            var displayItems = _searchResults.Count > 0 ? _searchResults : 
                              (_searchQuery.Length >= 1 ? Array.Empty<WorkItem>() : (IReadOnlyList<WorkItem>)_recentItems);
            if (displayItems.Count > 0)
            {
                builder.OpenElement(50, "div");
                builder.AddAttribute(51, "class", "workitem-dropdown");

                foreach (var item in displayItems.Take(5))
                {
                    var capturedItem = item;
                    builder.OpenElement(52, "div");
                    builder.AddAttribute(53, "class", $"workitem-dropdown-item{(capturedItem.Id == _activeItem?.Id ? " active" : "")}");
                    builder.AddAttribute(54, "onclick", EventCallback.Factory.Create(this, () =>
                    {
                        _activeItem = capturedItem;
                        _showSearch = false;
                        _searchQuery = string.Empty;
                        _searchResults = Array.Empty<WorkItem>();
                        OnStateChanged?.Invoke();
                    }));

                    builder.OpenElement(55, "span");
                    builder.AddAttribute(56, "class", "workitem-dropdown-id");
                    builder.AddContent(57, $"#{capturedItem.Id}");
                    builder.CloseElement();

                    builder.OpenElement(58, "span");
                    builder.AddAttribute(59, "class", "workitem-dropdown-title");
                    builder.AddContent(60, capturedItem.Title);
                    builder.CloseElement();

                    builder.OpenElement(61, "span");
                    builder.AddAttribute(62, "class", $"workitem-state workitem-state-{capturedItem.State.ToLowerInvariant().Replace(" ", "-")}");
                    builder.AddContent(63, capturedItem.State);
                    builder.CloseElement();

                    builder.CloseElement(); // workitem-dropdown-item
                }

                builder.CloseElement(); // workitem-dropdown
            }
            else if (_searchQuery.Length >= 1)
            {
                builder.OpenElement(70, "div");
                builder.AddAttribute(71, "class", "workitem-dropdown");
                builder.OpenElement(72, "div");
                builder.AddAttribute(73, "class", "workitem-no-results");
                builder.AddContent(74, "No items found");
                builder.CloseElement();
                builder.CloseElement();
            }

            builder.CloseElement(); // workitem-search
        }

        // Recent items list
        if (_recentItems.Count > 0 && !_showSearch)
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
