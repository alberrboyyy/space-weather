using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using SpaceWeather.App.ViewModels;
using SpaceWeather.App.Views;

namespace SpaceWeather.App;

public partial class App : Application
{
    // Charge App.axaml
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    // Appelé une fois qu'Avalonia a fini son initialisation interne, c'est ici qu'on crée la fenêtre de l'app
    public override void OnFrameworkInitializationCompleted()
    {
        // Vrai uniquement pour une appli desktop classique
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                // DataContext posé après le constructeur de MainWindow
                DataContext = new MainViewModel(),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}
