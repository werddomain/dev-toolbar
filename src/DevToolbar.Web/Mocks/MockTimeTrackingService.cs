namespace DevToolbar.Web.Mocks;

using DevToolbar.Core.Interfaces;
using DevToolbar.Core.Models;

/// <summary>
/// Mock time tracking service for web testing with idle detection support (US5.3).
/// </summary>
public class MockTimeTrackingService : ITimeTrackingService
{
    private TimeEntry? _activeEntry;
    private readonly List<TimeEntry> _entries = new();
    private DateTime _lastActivity = DateTime.Now;

    public event Action? OnTrackingChanged;

    public TimeSpan IdleTimeout => TimeSpan.FromMinutes(15);
    public bool IsIdlePaused { get; private set; }

    public void ReportActivity()
    {
        _lastActivity = DateTime.Now;
        if (IsIdlePaused && _activeEntry != null)
        {
            // Resume tracking - start a new entry continuing where we left off
            IsIdlePaused = false;
            OnTrackingChanged?.Invoke();
        }
    }

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
        _lastActivity = DateTime.Now;
        IsIdlePaused = false;
        OnTrackingChanged?.Invoke();
        return _activeEntry;
    }

    public TimeEntry? Stop()
    {
        if (_activeEntry == null) return null;

        _activeEntry.EndTime = DateTime.Now;
        var stopped = _activeEntry;
        _activeEntry = null;
        IsIdlePaused = false;
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
