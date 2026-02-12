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
    public event Action? OnIdleDetected;

    public TimeSpan IdleTimeout => TimeSpan.FromMinutes(15);
    public bool IsIdlePaused { get; private set; }

    // Simulated idle detection timer
    private Timer? _idleTimer;

    public MockTimeTrackingService()
    {
        // Pre-populate with sample time entries for Time Report demo (US5.4)
        var today = DateTime.Today;
        _entries.AddRange(new[]
        {
            new TimeEntry { ProjectId = "proj-webapi", WorkItemId = "1234", StartTime = today.AddHours(9), EndTime = today.AddHours(10).AddMinutes(30), Description = "Fix login redirect" },
            new TimeEntry { ProjectId = "proj-webapi", WorkItemId = "1235", StartTime = today.AddHours(10).AddMinutes(45), EndTime = today.AddHours(12), Description = "Dark mode research" },
            new TimeEntry { ProjectId = "proj-webapi", StartTime = today.AddHours(13), EndTime = today.AddHours(14).AddMinutes(15), Description = "Code review" },
            new TimeEntry { ProjectId = "proj-frontend", WorkItemId = "1236", StartTime = today.AddHours(14).AddMinutes(30), EndTime = today.AddHours(16), Description = "Update docs" },
            new TimeEntry { ProjectId = "proj-webapi", WorkItemId = "1234", StartTime = today.AddDays(-1).AddHours(9), EndTime = today.AddDays(-1).AddHours(11), Description = "Login page testing" },
            new TimeEntry { ProjectId = "proj-frontend", WorkItemId = "1235", StartTime = today.AddDays(-1).AddHours(13), EndTime = today.AddDays(-1).AddHours(15).AddMinutes(30), Description = "UI components" },
            new TimeEntry { ProjectId = "proj-devops", StartTime = today.AddDays(-2).AddHours(10), EndTime = today.AddDays(-2).AddHours(12), Description = "Pipeline config" },
        });
    }

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

        // Start idle detection timer (simulated for web testing)
        _idleTimer?.Dispose();
        _idleTimer = new Timer(_ =>
        {
            if (_activeEntry != null && !IsIdlePaused && (DateTime.Now - _lastActivity) >= IdleTimeout)
            {
                IsIdlePaused = true;
                OnIdleDetected?.Invoke();
                OnTrackingChanged?.Invoke();
            }
        }, null, IdleTimeout, TimeSpan.FromMinutes(1));

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
        _idleTimer?.Dispose();
        _idleTimer = null;
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
