using Microsoft.UI;
using Microsoft.UI.Windowing;
using WinRT.Interop;

namespace DevToolbar.Maui.Platforms.Windows;

/// <summary>
/// Windows-specific window configuration for DevToolbar (US8.1).
/// Handles borderless window, TopMost, and AppBar-like behavior.
/// </summary>
public static class WindowHelper
{
    /// <summary>
    /// Configure the MAUI window for toolbar mode:
    /// - Remove title bar
    /// - Set always on top
    /// - Set compact size
    /// </summary>
    public static void ConfigureToolbarWindow(Microsoft.Maui.Controls.Window mauiWindow, bool topMost = true)
    {
        // Access the native WinUI window
        var nativeWindow = mauiWindow.Handler?.PlatformView as Microsoft.UI.Xaml.Window;
        if (nativeWindow == null) return;

        var hWnd = WindowNative.GetWindowHandle(nativeWindow);
        var windowId = Win32Interop.GetWindowIdFromWindow(hWnd);
        var appWindow = AppWindow.GetFromWindowId(windowId);

        if (appWindow == null) return;

        // US8.1: Remove title bar
        if (appWindow.Presenter is OverlappedPresenter presenter)
        {
            presenter.SetBorderAndTitleBar(hasBorder: false, hasTitleBar: false);
            presenter.IsAlwaysOnTop = topMost;
            presenter.IsResizable = true;
            presenter.IsMinimizable = false;
        }

        // Set initial size — wide floating pill toolbar
        appWindow.Resize(new global::Windows.Graphics.SizeInt32(1100, 80));
    }
}
