using DevToolbar.Core.Events;
using DevToolbar.Core.Interfaces;
using DevToolbar.Plugins.Git;
using DevToolbar.Plugins.Services;
using DevToolbar.UI.Services;
using DevToolbar.Web.Mocks;

var builder = WebApplication.CreateBuilder(args);

// Add Blazor services
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Register mock services for web testing
builder.Services.AddSingleton<IProcessService, MockProcessService>();
builder.Services.AddSingleton<IFileSystemService, MockFileSystemService>();
builder.Services.AddSingleton<ISettingsService, MockSettingsService>();

// Register plugins
builder.Services.AddSingleton<IPlugin, GitPlugin>();
builder.Services.AddSingleton<IPluginLoader, PluginLoader>();

// Register UI services
builder.Services.AddSingleton<ThemeService>();
builder.Services.AddSingleton<EventAggregator>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<DevToolbar.Web.Components.App>()
    .AddInteractiveServerRenderMode()
    .AddAdditionalAssemblies(typeof(DevToolbar.UI.Pages.Home).Assembly);

app.Run();

public partial class Program { }
