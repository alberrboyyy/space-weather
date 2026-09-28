using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using SpaceWeather.App.ViewModels;
using SpaceWeather.Core.Import;

namespace SpaceWeather.App.Views;

public partial class MainWindow : Window
{
    // Raccourci pour éviter de caster DataContext partout. Le ! garanti au compilateur que la valeur ne sera pas nulle
    private MainViewModel ViewModel => (MainViewModel)DataContext!;

    public MainWindow()
    {
        InitializeComponent();

        // A cet instant DataContext est encore null on attend donc DataContextChanged
        // pour s'abonner au PlotInvalidated du ViewModel une seule fois.
        DataContextChanged += (_, _) =>
        {
            if (DataContext is MainViewModel vm)
                vm.PlotInvalidated += (_, _) => RefreshPlot();
        };
    }

    // Relié au Click du bouton "Importer des données" dans le xaml
    private async void OnImportClick(object? sender, RoutedEventArgs e)
    {
        // Ouvre le sélecteur de fichier du système
        var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Importer un fichier NOAA (JSON)",
            AllowMultiple = false,
            FileTypeFilter = [new FilePickerFileType("Fichier JSON") { Patterns = ["*.json"] }]
        });

        // FirstOrDefault plutôt que First : si l'utilisateur annule, files est vide, on récupère null
        var file = files.FirstOrDefault();
        if (file is null)
            return;

        // file.Path est une URI, LocalPath donne un vrai chemin disque
        var series = NoaaProtonFileImporter.ImportFromFile(file.Path.LocalPath);
        // Relai des TimeSeries au ViewModel
        ViewModel.LoadSeries(series);
    }

    private void RefreshPlot()
    {
        // Efface les anciennes courbes, sinon un second import les empilerait sur les nouvelles
        PlotControl.Plot.Clear();

        // Seules les séries actuellement cochées sont dessinées : c'est ici,
        // que l'état IsChecked d'une checkbox influence vraiment l'affichage
        foreach (var toggle in ViewModel.Series.Where(t => t.IsChecked))
        {
            // ScottPlot attend des tableaux de double, pas des TimeSeriesPoint :
            // ToOADate() convertit chaque DateTime en nombre
            var xs = toggle.Series.Points.Select(p => p.Timestamp.ToOADate()).ToArray();
            var ys = toggle.Series.Points.Select(p => p.Value).ToArray();
            // Ajoute la courbe et l'étiquette dans la légende avec le nom de la série
            PlotControl.Plot.Add.ScatterLine(xs, ys).LegendText = toggle.Name;
        }

        // Affiche les dates lisibles sur l'axe du bas plutôt que les nombres bruts de ToOADate()
        PlotControl.Plot.Axes.DateTimeTicksBottom();
        PlotControl.Plot.ShowLegend();
        // Force le contrôle à se redessiner à l'écran avec les nouvelles données
        PlotControl.Refresh();
    }
}
