using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using SpaceWeather.App.ViewModels;
using SpaceWeather.App.Views;
using SpaceWeather.Core.Storage;

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
            var viewModel = new MainViewModel();

            // Reload des points ajoutés dans le passé
            var stored = LocalSeriesStore.Load(AppPaths.SeriesFile);
            if (stored.Count > 0)
                viewModel.LoadSeries(stored);

            desktop.MainWindow = new MainWindow
            {
                // DataContext posé après le constructeur de MainWindow
                DataContext = viewModel,
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}
