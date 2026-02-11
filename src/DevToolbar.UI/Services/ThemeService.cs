namespace DevToolbar.UI.Services;

using DevToolbar.Core.Models;

/// <summary>
/// Generates CSS variables from the active project's theme configuration.
/// </summary>
public class ThemeService
{
    private ThemeConfig _currentTheme = new();

    public void SetTheme(ThemeConfig theme)
    {
        _currentTheme = theme;
        OnThemeChanged?.Invoke();
    }

    public string GetCssVariables() =>
        $"--accent-color: {_currentTheme.AccentColor}; --font-family: {_currentTheme.FontFamily};";

    public event Action? OnThemeChanged;
}
