namespace DevToolbar.Web.Mocks;

using DevToolbar.Core.Interfaces;
using DevToolbar.Core.Models;

/// <summary>
/// Mock work item provider for web testing with sample data.
/// </summary>
public class MockWorkItemProvider : IWorkItemProvider
{
    public string ProviderName => "Mock";

    private readonly List<WorkItem> _items = new()
    {
        new WorkItem { Id = "1234", Title = "Fix login page redirect", State = "Active", Type = "Bug", AssignedTo = "dev@example.com", Url = "#" },
        new WorkItem { Id = "1235", Title = "Add dark mode support", State = "In Progress", Type = "User Story", AssignedTo = "dev@example.com", Url = "#" },
        new WorkItem { Id = "1236", Title = "Update API documentation", State = "New", Type = "Task", AssignedTo = "dev@example.com", Url = "#" }
    };

    public Task<IReadOnlyList<WorkItem>> SearchAsync(string query) =>
        Task.FromResult<IReadOnlyList<WorkItem>>(_items.AsReadOnly());

    public Task<WorkItem?> GetByIdAsync(string id) =>
        Task.FromResult(_items.FirstOrDefault(i => i.Id == id));

    public string GetWebUrl(WorkItem item) => item.Url;
}
