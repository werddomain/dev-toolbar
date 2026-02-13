using Microsoft.AspNetCore.Components.WebView.Maui;

namespace DevToolbar.Maui;

/// <summary>
/// Main page hosting the Blazor WebView.
/// </summary>
public class MainPage : ContentPage
{
    public MainPage()
    {
        Content = new BlazorWebView
        {
            HostPage = "wwwroot/index.html",
            RootComponents =
            {
                new RootComponent
                {
                    Selector = "#app",
                    ComponentType = typeof(DevToolbar.UI.Pages.Home)
                }
            }
        };
    }
}
