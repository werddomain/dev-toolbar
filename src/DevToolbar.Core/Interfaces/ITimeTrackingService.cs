namespace DevToolbar.Core.Interfaces;

using DevToolbar.Core.Models;

/// <summary>
/// Service for tracking time entries.
/// </summary>
public interface ITimeTrackingService
{
    /// <summary>Start tracking time for a project.</summary>
    TimeEntry Start(string projectId, string? workItemId = null);

    /// <summary>Stop the current active entry.</summary>
    TimeEntry? Stop();

    /// <summary>Get the currently active time entry, if any.</summary>
    TimeEntry? GetActive();

    /// <summary>Get time entries for a project.</summary>
    Task<IReadOnlyList<TimeEntry>> GetEntriesAsync(string projectId);

    /// <summary>Get total tracked time for a project today.</summary>
    TimeSpan GetTodayTotal(string projectId);

    /// <summary>Event fired when tracking state changes.</summary>
    event Action? OnTrackingChanged;
}
