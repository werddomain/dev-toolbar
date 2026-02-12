namespace DevToolbar.Plugins.TimeTracker;

using DevToolbar.Core.Interfaces;
using DevToolbar.Core.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

/// <summary>
/// Plugin for tracking time spent on projects with idle detection (US5.3).
/// </summary>
public class TimeTrackerPlugin : IPlugin
{
    private readonly ITimeTrackingService _timeService;

    public string UniqueId => "time-tracker";
    public string Name => "Time Tracker";
    public string Icon => "⏱";
    public bool IsEnabled { get; set; } = true;
    public event Action? OnStateChanged;

    private string _currentProjectId = string.Empty;

    public TimeTrackerPlugin(ITimeTrackingService timeService)
    {
        _timeService = timeService;
    }

    public Task OnProjectChangedAsync(PluginContext context)
    {
        _currentProjectId = context.Project.Id;
        return Task.CompletedTask;
    }

    public RenderFragment? Render() => builder =>
    {
        var active = _timeService.GetActive();
        var isRunning = active != null && active.ProjectId == _currentProjectId;
        var todayTotal = _timeService.GetTodayTotal(_currentProjectId);
        var isIdle = _timeService.IsIdlePaused;

        builder.OpenElement(0, "div");
        builder.AddAttribute(1, "class", "plugin-timetracker");

        // Timer controls row
        builder.OpenElement(2, "div");
        builder.AddAttribute(3, "class", "timer-controls");

        // Start/Stop button
        builder.OpenElement(4, "button");
        builder.AddAttribute(5, "class", isRunning ? "timer-btn timer-btn-stop" : "timer-btn timer-btn-start");
        builder.AddAttribute(6, "onclick", EventCallback.Factory.Create(this, () =>
        {
            if (isRunning)
                _timeService.Stop();
            else
                _timeService.Start(_currentProjectId);
            OnStateChanged?.Invoke();
        }));
        builder.AddContent(7, isRunning ? "⏹ Stop" : "▶ Start");
        builder.CloseElement(); // button

        // Status indicator
        builder.OpenElement(8, "span");
        if (isIdle && isRunning)
        {
            builder.AddAttribute(9, "class", "timer-status idle");
            builder.AddContent(10, "💤 Idle");
        }
        else
        {
            builder.AddAttribute(9, "class", isRunning ? "timer-status running" : "timer-status stopped");
            builder.AddContent(10, isRunning ? "⏺ Recording" : "⏸ Stopped");
        }
        builder.CloseElement();

        builder.CloseElement(); // timer-controls

        // Timer display
        builder.OpenElement(11, "div");
        builder.AddAttribute(12, "class", "timer-display");

        builder.OpenElement(13, "span");
        builder.AddAttribute(14, "class", "timer-duration");
        if (isRunning && active != null)
        {
            builder.AddContent(15, FormatDuration(active.Duration));
        }
        else
        {
            builder.AddContent(15, "00:00:00");
        }
        builder.CloseElement();

        builder.CloseElement(); // timer-display

        // Idle timeout info
        builder.OpenElement(25, "div");
        builder.AddAttribute(26, "class", "timer-idle-info");
        builder.OpenElement(27, "span");
        builder.AddAttribute(28, "class", "timer-idle-label");
        builder.AddContent(29, $"Idle timeout: {(int)_timeService.IdleTimeout.TotalMinutes} min");
        builder.CloseElement();
        builder.CloseElement(); // timer-idle-info

        // Today's total
        builder.OpenElement(16, "div");
        builder.AddAttribute(17, "class", "timer-today");

        builder.OpenElement(18, "span");
        builder.AddAttribute(19, "class", "timer-today-label");
        builder.AddContent(20, "Today:");
        builder.CloseElement();

        builder.OpenElement(21, "span");
        builder.AddAttribute(22, "class", "timer-today-value");
        builder.AddContent(23, FormatDuration(todayTotal));
        builder.CloseElement();

        builder.CloseElement(); // timer-today

        builder.CloseElement(); // plugin-timetracker
    };

    private static string FormatDuration(TimeSpan duration) =>
        $"{(int)duration.TotalHours:D2}:{duration.Minutes:D2}:{duration.Seconds:D2}";
}
