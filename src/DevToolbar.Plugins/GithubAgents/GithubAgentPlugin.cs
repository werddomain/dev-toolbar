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

        // Status badge
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
        }

        builder.CloseElement(); // cicd-header

        // Sessions list
        builder.OpenElement(13, "div");
        builder.AddAttribute(14, "class", "cicd-sessions");

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

            builder.OpenElement(15, "div");
            builder.AddAttribute(16, "class", $"cicd-session {statusClass}{(session.IsRead ? "" : " unread")}");

            builder.OpenElement(17, "span");
            builder.AddAttribute(18, "class", "cicd-session-icon");
            builder.AddContent(19, statusIcon);
            builder.CloseElement();

            builder.OpenElement(20, "a");
            builder.AddAttribute(21, "href", session.Url);
            builder.AddAttribute(22, "target", "_blank");
            builder.AddAttribute(23, "class", "cicd-session-name");
            builder.AddContent(24, session.Name);
            builder.CloseElement();

            builder.OpenElement(25, "span");
            builder.AddAttribute(26, "class", "cicd-session-branch");
            builder.AddContent(27, session.Branch);
            builder.CloseElement();

            builder.CloseElement(); // cicd-session
        }

        builder.CloseElement(); // cicd-sessions
        builder.CloseElement(); // plugin-cicd
    };
}
