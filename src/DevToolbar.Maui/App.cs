namespace DevToolbar.Maui;

/// <summary>
/// MAUI Application class.
/// </summary>
public partial class App : Application
{
    public App()
    {
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var window = new Window(new MainPage())
        {
            Title = "DevToolbar",
            Width = 900,
            Height = 600,
        };

        return window;
    }
}
