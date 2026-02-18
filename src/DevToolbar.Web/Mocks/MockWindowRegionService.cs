using DevToolbar.Core.Interfaces;

namespace DevToolbar.Web.Mocks;

/// <summary>
/// Mock IWindowRegionService for web testing — no-ops (browser has no window region).
/// </summary>
public class MockWindowRegionService : IWindowRegionService
{
    public void ExpandRegion(int totalHeightCss) { }
    public void CollapseRegion() { }
}
