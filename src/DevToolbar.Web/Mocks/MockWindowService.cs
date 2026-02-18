using DevToolbar.Core.Interfaces;
using DevToolbar.Core.Models;

namespace DevToolbar.Web.Mocks;

/// <summary>
/// Mock IWindowService for web testing — returns empty data.
/// </summary>
public class MockWindowService : IWindowService
{
    public Task<List<WindowInfo>> GetOpenWindowsOnMonitorAsync(int monitorIndex)
        => Task.FromResult(new List<WindowInfo>());

    public void ActivateWindow(nint hWnd)
    {
        // No-op in web context
    }

    public List<MonitorInfo> GetMonitors()
        => new()
        {
            new MonitorInfo
            {
                Index = 0,
                DeviceName = "DISPLAY1",
                IsPrimary = true,
                Bounds = new ScreenRect { Left = 0, Top = 0, Right = 1920, Bottom = 1080 },
                WorkArea = new ScreenRect { Left = 0, Top = 0, Right = 1920, Bottom = 1040 }
            }
        };
}
