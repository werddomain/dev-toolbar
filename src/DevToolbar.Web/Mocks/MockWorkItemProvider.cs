namespace DevToolbar.Web.Mocks;

using DevToolbar.Core.Interfaces;
using DevToolbar.Core.Models;

/// <summary>
/// Mock work item provider for web testing with sample data and search support.
/// </summary>
public class MockWorkItemProvider : IWorkItemProvider
{
    public string ProviderName => "Mock";

    private readonly List<WorkItem> _items = new()
    {
        new WorkItem { Id = "1234", Title = "Fix login page redirect", State = "Active", Type = "Bug", AssignedTo = "dev@example.com", Url = "#" },
        new WorkItem { Id = "1235", Title = "Add dark mode support", State = "In Progress", Type = "User Story", AssignedTo = "dev@example.com", Url = "#" },
        new WorkItem { Id = "1236", Title = "Update API documentation", State = "New", Type = "Task", AssignedTo = "dev@example.com", Url = "#" },
        new WorkItem { Id = "1237", Title = "Refactor authentication module", State = "New", Type = "Task", AssignedTo = "admin@example.com", Url = "#" },
        new WorkItem { Id = "1238", Title = "Fix dashboard performance", State = "Active", Type = "Bug", AssignedTo = "dev@example.com", Url = "#" }
    };

    public Task<IReadOnlyList<WorkItem>> SearchAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return Task.FromResult<IReadOnlyList<WorkItem>>(_items.Take(3).ToList().AsReadOnly());

        var filtered = _items
            .Where(i => i.Title.Contains(query, StringComparison.OrdinalIgnoreCase)
                     || i.Id.Contains(query, StringComparison.OrdinalIgnoreCase)
                     || i.Type.Contains(query, StringComparison.OrdinalIgnoreCase))
            .ToList()
            .AsReadOnly();

        return Task.FromResult<IReadOnlyList<WorkItem>>(filtered.Count > 0 ? filtered : _items.Take(3).ToList().AsReadOnly());
    }

    public Task<WorkItem?> GetByIdAsync(string id) =>
        Task.FromResult(_items.FirstOrDefault(i => i.Id == id));

    public string GetWebUrl(WorkItem item) => item.Url;
}
