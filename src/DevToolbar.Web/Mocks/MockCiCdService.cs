namespace DevToolbar.Web.Mocks;

using DevToolbar.Core.Interfaces;
using DevToolbar.Core.Models;

/// <summary>
/// Mock CI/CD service with sample GitHub Actions data.
/// </summary>
public class MockCiCdService : ICiCdService
{
    private readonly List<CiCdSession> _sessions;

    public event Action? OnSessionsUpdated;

    public MockCiCdService()
    {
        _sessions = new List<CiCdSession>
        {
            new CiCdSession
            {
                Id = "run-101",
                Name = "Build & Test",
                Status = CiCdStatus.Completed,
                Conclusion = "success",
                StartedAt = DateTime.Now.AddHours(-2),
                CompletedAt = DateTime.Now.AddHours(-1).AddMinutes(-45),
                Url = "#",
                Branch = "main",
                IsRead = false
            },
            new CiCdSession
            {
                Id = "run-102",
                Name = "Deploy to Staging",
                Status = CiCdStatus.Completed,
                Conclusion = "failure",
                StartedAt = DateTime.Now.AddHours(-1),
                CompletedAt = DateTime.Now.AddMinutes(-30),
                Url = "#",
                Branch = "feature/login",
                IsRead = false
            },
            new CiCdSession
            {
                Id = "run-103",
                Name = "Code Analysis",
                Status = CiCdStatus.InProgress,
                StartedAt = DateTime.Now.AddMinutes(-5),
                Url = "#",
                Branch = "main",
                IsRead = false
            }
        };
    }

    public Task<IReadOnlyList<CiCdSession>> GetSessionsAsync(string projectId) =>
        Task.FromResult<IReadOnlyList<CiCdSession>>(_sessions.AsReadOnly());

    public int GetUnreadCount(string projectId) =>
        _sessions.Count(s => !s.IsRead && s.Status == CiCdStatus.Completed);

    public void MarkAsRead(string sessionId)
    {
        var session = _sessions.FirstOrDefault(s => s.Id == sessionId);
        if (session != null)
        {
            session.IsRead = true;
            OnSessionsUpdated?.Invoke();
        }
    }

    public void MarkAllAsRead(string projectId)
    {
        foreach (var session in _sessions)
        {
            session.IsRead = true;
        }
        OnSessionsUpdated?.Invoke();
    }
}
