using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using SpaceWeather.App.Extensions;
using SpaceWeather.App.ViewModels;
using SpaceWeather.Core.Import;
using SpaceWeather.Core.Models;
using SpaceWeather.Core.Storage;

namespace SpaceWeather.App.Views;

public partial class MainWindow : Window
{
    // Intervale des mesures NOAA
    private static readonly TimeSpan SampleInterval = TimeSpan.FromMinutes(5);

    // Fenêtre la plus large disponible sur l'API temps réel de la NOAA (pas d'historique complet)
    private const string NoaaLiveUrl = "https://services.swpc.noaa.gov/json/goes/primary/integral-protons-7-day.json";

    // Un seul HttpClient réutilisé pour toute la durée de vie de la fenêtre (recommandé par .NET,
    // en créer un nouveau à chaque appel peut épuiser les sockets disponibles)
    private static readonly HttpClient HttpClient = new();

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
            {
                vm.PlotInvalidated += (_, _) => RefreshPlot();
                RefreshPlot();
            }
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

        try
        {
            // file.Path est une URI, LocalPath donne un vrai chemin disque
            var imported = NoaaProtonFileImporter.ImportFromFile(file.Path.LocalPath);
            ApplyImportedSeries(imported);
        }
        catch (Exception ex)
        {
            ViewModel.ErrorMessage = $"Échec de l'import : {ex.Message}";
        }
    }

    // Relié au Click du bouton "Récupérer depuis la NOAA" dans le xaml
    private async void OnPullClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            var json = await HttpClient.GetStringAsync(NoaaLiveUrl);
            var imported = NoaaProtonFileImporter.ImportFromJson(json);
            ApplyImportedSeries(imported);
        }
        catch (Exception ex)
        {
            // Pas de wifi, NOAA indisponible, JSON invalide, etc. : message affiché, pas de crash
            ViewModel.ErrorMessage = $"Échec de la récupération NOAA : {ex.Message}";
        }
    }

    // Fusionne les nouvelles séries avec celles déjà affichées, sauvegarde, recharge le ViewModel.
    // Commun aux deux boutons (import fichier et récupération NOAA).
    private void ApplyImportedSeries(List<TimeSeries> imported)
    {
        var current = ViewModel.Series.Select(t => t.Series).ToList();
        var merged = LocalSeriesStore.Merge(current, imported);

        LocalSeriesStore.Save(AppPaths.SeriesFile, merged);
        ViewModel.LoadSeries(merged);
        ViewModel.ErrorMessage = null;
    }

    private void RefreshPlot()
    {
        // Efface les anciennes courbes, sinon un second import les empilerait sur les nouvelles
        PlotControl.Plot.Clear();

        // Seules les séries actuellement cochées sont dessinées : c'est ici,
        // que l'état IsChecked d'une checkbox influence vraiment l'affichage
        foreach (var toggle in ViewModel.Series.Where(t => t.IsChecked))
        {
            // ToPlotArrays coupe la ligne dès que l'écart dépasse la cadence normale
            var (xs, ys) = toggle.Series.Points.ToPlotArrays(SampleInterval);
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
