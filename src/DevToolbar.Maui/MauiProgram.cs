using DevToolbar.Core.Events;
using DevToolbar.Core.Interfaces;
using DevToolbar.Maui.Services;
using DevToolbar.Plugins.Git;
using DevToolbar.Plugins.GithubAgents;
using DevToolbar.Plugins.Services;
using DevToolbar.Plugins.TimeTracker;
using DevToolbar.Plugins.WorkItems;
using DevToolbar.UI.Services;
using Microsoft.Extensions.Logging;

namespace DevToolbar.Maui;

/// <summary>
/// MAUI application entry point with real service registrations.
/// </summary>
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        builder.Services.AddMauiBlazorWebView();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        // Register REAL native services
        builder.Services.AddSingleton<IProcessService, WindowsProcessService>();
        builder.Services.AddSingleton<INativeService, WindowsNativeService>();
        // TODO: Add SQLiteStorageService when SQLite package is added
        // builder.Services.AddSingleton<IStorageService, SqliteStorageService>();
        builder.Services.AddSingleton<IFileSystemService, WindowsFileSystemService>();
        builder.Services.AddSingleton<ISettingsService, JsonSettingsService>();
        builder.Services.AddSingleton<IScriptService, WindowsScriptService>();

        // Register plugins
        builder.Services.AddSingleton<IPlugin, GitPlugin>();
        builder.Services.AddSingleton<IWorkItemProvider, MockWorkItemProvider>();
        builder.Services.AddSingleton<IPlugin, WorkItemsPlugin>();
        builder.Services.AddSingleton<ITimeTrackingService, MockTimeTrackingService>();
        builder.Services.AddSingleton<IPlugin, TimeTrackerPlugin>();
        builder.Services.AddSingleton<ICiCdService, MockCiCdService>();
        builder.Services.AddSingleton<IPlugin, GithubAgentPlugin>();
        builder.Services.AddSingleton<IPluginLoader, PluginLoader>();

        // Register UI services
        builder.Services.AddSingleton<INotificationService, NotificationService>();
        builder.Services.AddSingleton<ThemeService>();
        builder.Services.AddSingleton<EventAggregator>();

        return builder.Build();
    }
}
