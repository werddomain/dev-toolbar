namespace DevToolbar.Web.Mocks;

using DevToolbar.Core.Interfaces;
using DevToolbar.Core.Models;

/// <summary>
/// Mock time tracking service for web testing.
/// </summary>
public class MockTimeTrackingService : ITimeTrackingService
{
    private TimeEntry? _activeEntry;
    private readonly List<TimeEntry> _entries = new();

    public event Action? OnTrackingChanged;

    public TimeEntry Start(string projectId, string? workItemId = null)
    {
        // Stop current if running
        Stop();

        _activeEntry = new TimeEntry
        {
            ProjectId = projectId,
            WorkItemId = workItemId,
            StartTime = DateTime.Now
        };
        _entries.Add(_activeEntry);
        OnTrackingChanged?.Invoke();
        return _activeEntry;
    }

    public TimeEntry? Stop()
    {
        if (_activeEntry == null) return null;

        _activeEntry.EndTime = DateTime.Now;
        var stopped = _activeEntry;
        _activeEntry = null;
        OnTrackingChanged?.Invoke();
        return stopped;
    }

    public TimeEntry? GetActive() => _activeEntry;

    public Task<IReadOnlyList<TimeEntry>> GetEntriesAsync(string projectId) =>
        Task.FromResult<IReadOnlyList<TimeEntry>>(
            _entries.Where(e => e.ProjectId == projectId).ToList().AsReadOnly());

    public TimeSpan GetTodayTotal(string projectId)
    {
        var today = DateTime.Today;
        return _entries
            .Where(e => e.ProjectId == projectId && e.StartTime.Date == today)
            .Aggregate(TimeSpan.Zero, (total, entry) => total + entry.Duration);
    }
}
