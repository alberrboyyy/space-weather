using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using SpaceWeather.App.ViewModels;
using SpaceWeather.Core.Import;

namespace SpaceWeather.App.Views;

public partial class MainWindow : Window
{
    private MainViewModel ViewModel => (MainViewModel)DataContext!;

    public MainWindow()
    {
        InitializeComponent();

        DataContextChanged += (_, _) =>
        {
            if (DataContext is MainViewModel vm)
                vm.PlotInvalidated += (_, _) => RefreshPlot();
        };
    }

    private async void OnImportClick(object? sender, RoutedEventArgs e)
    {
        var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Importer un fichier NOAA (JSON)",
            AllowMultiple = false,
            FileTypeFilter = [new FilePickerFileType("Fichier JSON") { Patterns = ["*.json"] }]
        });

        var file = files.FirstOrDefault();
        if (file is null)
            return;

        var series = NoaaProtonFileImporter.ImportFromFile(file.Path.LocalPath);
        ViewModel.LoadSeries(series);
    }

    private void RefreshPlot()
    {
        PlotControl.Plot.Clear();

        foreach (var toggle in ViewModel.Series.Where(t => t.IsChecked))
        {
            var xs = toggle.Series.Points.Select(p => p.Timestamp.ToOADate()).ToArray();
            var ys = toggle.Series.Points.Select(p => p.Value).ToArray();
            PlotControl.Plot.Add.ScatterLine(xs, ys).LegendText = toggle.Name;
        }

        PlotControl.Plot.Axes.DateTimeTicksBottom();
        PlotControl.Plot.ShowLegend();
        PlotControl.Refresh();
    }
}
