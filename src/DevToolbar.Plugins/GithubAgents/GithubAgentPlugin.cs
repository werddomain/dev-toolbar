namespace DevToolbar.Plugins.GithubAgents;

using DevToolbar.Core.Interfaces;
using DevToolbar.Core.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

/// <summary>
/// Plugin for monitoring GitHub Actions / CI/CD sessions.
/// </summary>
public class GithubAgentPlugin : IPlugin
{
    private readonly ICiCdService _ciCdService;

    public string UniqueId => "github-agents";
    public string Name => "CI/CD Status";
    public string Icon => "🔄";
    public bool IsEnabled { get; set; } = true;
    public event Action? OnStateChanged;

    private string _currentProjectId = string.Empty;
    private IReadOnlyList<CiCdSession> _sessions = Array.Empty<CiCdSession>();

    public GithubAgentPlugin(ICiCdService ciCdService)
    {
        _ciCdService = ciCdService;
    }

    public async Task OnProjectChangedAsync(PluginContext context)
    {
        _currentProjectId = context.Project.Id;
        _sessions = await _ciCdService.GetSessionsAsync(_currentProjectId);
    }

    public RenderFragment? Render() => builder =>
    {
        var unreadCount = _ciCdService.GetUnreadCount(_currentProjectId);

        builder.OpenElement(0, "div");
        builder.AddAttribute(1, "class", "plugin-cicd");

        // Status badge header
        builder.OpenElement(2, "div");
        builder.AddAttribute(3, "class", "cicd-header");

        builder.OpenElement(4, "span");
        builder.AddAttribute(5, "class", "cicd-icon");
        builder.AddContent(6, "🔄");
        builder.CloseElement();

        builder.OpenElement(7, "span");
        builder.AddAttribute(8, "class", "cicd-label");
        builder.AddContent(9, "Pipelines");
        builder.CloseElement();

        if (unreadCount > 0)
        {
            builder.OpenElement(10, "span");
            builder.AddAttribute(11, "class", "cicd-badge");
            builder.AddContent(12, unreadCount.ToString());
            builder.CloseElement();

            // Mark All Read button
            builder.OpenElement(13, "button");
            builder.AddAttribute(14, "class", "cicd-mark-all-read");
            builder.AddAttribute(15, "title", "Mark all as read");
            builder.AddAttribute(16, "onclick", EventCallback.Factory.Create(this, () =>
            {
                _ciCdService.MarkAllAsRead(_currentProjectId);
                OnStateChanged?.Invoke();
            }));
            builder.AddContent(17, "✓ Read all");
            builder.CloseElement();
        }

        builder.CloseElement(); // cicd-header

        // Sessions list
        builder.OpenElement(18, "div");
        builder.AddAttribute(19, "class", "cicd-sessions");

        foreach (var session in _sessions.Take(5))
        {
            var statusClass = session.Status switch
            {
                CiCdStatus.Completed when session.Conclusion == "success" => "success",
                CiCdStatus.Completed when session.Conclusion == "failure" => "failure",
                CiCdStatus.InProgress => "running",
                _ => "queued"
            };

            var statusIcon = session.Status switch
            {
                CiCdStatus.Completed when session.Conclusion == "success" => "✅",
                CiCdStatus.Completed when session.Conclusion == "failure" => "❌",
                CiCdStatus.InProgress => "🔄",
                _ => "⏳"
            };

            builder.OpenElement(20, "div");
            builder.AddAttribute(21, "class", $"cicd-session {statusClass}{(session.IsRead ? "" : " unread")}");
            // Click to mark as read
            builder.AddAttribute(22, "onclick", EventCallback.Factory.Create(this, () =>
            {
                if (!session.IsRead)
                {
                    _ciCdService.MarkAsRead(session.Id);
                    OnStateChanged?.Invoke();
                }
            }));

            builder.OpenElement(23, "span");
            builder.AddAttribute(24, "class", "cicd-session-icon");
            builder.AddContent(25, statusIcon);
            builder.CloseElement();

            builder.OpenElement(26, "a");
            builder.AddAttribute(27, "href", session.Url);
            builder.AddAttribute(28, "target", "_blank");
            builder.AddAttribute(29, "class", "cicd-session-name");
            builder.AddContent(30, session.Name);
            builder.CloseElement();

            builder.OpenElement(31, "span");
            builder.AddAttribute(32, "class", "cicd-session-branch");
            builder.AddContent(33, session.Branch);
            builder.CloseElement();

            builder.CloseElement(); // cicd-session
        }

        builder.CloseElement(); // cicd-sessions
        builder.CloseElement(); // plugin-cicd
    };
}
