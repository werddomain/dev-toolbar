namespace DevToolbar.Plugins.TimeTracker;

using DevToolbar.Core.Interfaces;
using DevToolbar.Core.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

/// <summary>
/// Plugin for tracking time spent on projects.
/// </summary>
public class TimeTrackerPlugin : IPlugin
{
    private readonly ITimeTrackingService _timeService;

    public string UniqueId => "time-tracker";
    public string Name => "Time Tracker";
    public string Icon => "⏱";
    public bool IsEnabled { get; set; } = true;

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

        builder.OpenElement(0, "div");
        builder.AddAttribute(1, "class", "plugin-timetracker");

        // Timer display
        builder.OpenElement(2, "div");
        builder.AddAttribute(3, "class", "timer-display");

        builder.OpenElement(4, "span");
        builder.AddAttribute(5, "class", isRunning ? "timer-status running" : "timer-status stopped");
        builder.AddContent(6, isRunning ? "⏺ Recording" : "⏸ Stopped");
        builder.CloseElement();

        builder.OpenElement(7, "span");
        builder.AddAttribute(8, "class", "timer-duration");
        if (isRunning && active != null)
        {
            builder.AddContent(9, FormatDuration(active.Duration));
        }
        else
        {
            builder.AddContent(9, "00:00:00");
        }
        builder.CloseElement();

        builder.CloseElement(); // timer-display

        // Today's total
        builder.OpenElement(10, "div");
        builder.AddAttribute(11, "class", "timer-today");

        builder.OpenElement(12, "span");
        builder.AddAttribute(13, "class", "timer-today-label");
        builder.AddContent(14, "Today:");
        builder.CloseElement();

        builder.OpenElement(15, "span");
        builder.AddAttribute(16, "class", "timer-today-value");
        builder.AddContent(17, FormatDuration(todayTotal));
        builder.CloseElement();

        builder.CloseElement(); // timer-today

        builder.CloseElement(); // plugin-timetracker
    };

    private static string FormatDuration(TimeSpan duration) =>
        $"{(int)duration.TotalHours:D2}:{duration.Minutes:D2}:{duration.Seconds:D2}";
}
