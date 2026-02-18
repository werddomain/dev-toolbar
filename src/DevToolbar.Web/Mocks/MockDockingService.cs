using DevToolbar.Core.Interfaces;
using DevToolbar.Core.Models;

namespace DevToolbar.Web.Mocks;

/// <summary>
/// Mock IDockingService for web testing — all operations are no-ops.
/// </summary>
public class MockDockingService : IDockingService
{
    public bool IsDocked => false;

    public void DockWindow(DockEdge edge, int monitorIndex, int heightPixels) { }

    public void Undock() { }

    public void PositionOnMonitor(DockEdge edge, int monitorIndex, int heightPixels) { }

    public void ApplySettings(GlobalSettings settings) { }
}
