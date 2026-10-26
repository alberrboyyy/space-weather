using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using SpaceWeather.Core.Extensions;
using SpaceWeather.App.ViewModels;
using SpaceWeather.Core.Import;
using SpaceWeather.Core.Models;
using SpaceWeather.Core.Storage;
using ScottPlot.AxisRules;
using ScottPlot.Interactivity;
using ScottPlot.Interactivity.UserActionResponses;

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

    // Première et dernière date des séries cochées (format ToOADate), bornes de la scrollbar
    private double _dataMinX;
    private double _dataMaxX;

    public MainWindow()
    {
        InitializeComponent();

        // Par défaut le double-clic de ScottPlot affiche un benchmark, on le remplace par un retour à la vue complète
        PlotControl.UserInputProcessor.RemoveAll<DoubleClickBenchmark>();
        PlotControl.UserInputProcessor.UserActionResponses.Add(
            new DoubleClickResponse(StandardMouseButtons.Left, (_, _) => PlotControl.Plot.Axes.AutoScale()));

        // Déclenché à chaque changement de vue (molette, glissé, double-clic, scrollbar) pour garder la scrollbar à jour.
        // ScottPlot dessine sur le thread de rendu d'Avalonia, or les contrôles ne peuvent être modifiés que depuis le thread de l'UI :
        // Post renvoie la mise à jour sur le bon thread
        PlotControl.Plot.RenderManager.AxisLimitsChanged += (_, _) => Dispatcher.UIThread.Post(UpdateScrollBar);

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

    // TODO : la logique d'import est dans le code-behind, elle devrait être dans le ViewModel pour respecter MVVM
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
            // TODO : lecture synchrone sur le thread de l'UI, un gros fichier bloquerait la fenêtre
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
        var legend = new List<LegendEntry>();
        foreach (var toggle in ViewModel.Series.Where(t => t.IsChecked))
        {
            // ToPlotArrays coupe la ligne dès que l'écart dépasse la cadence normale
            var (xs, ys) = toggle.Series.Points.ToPlotArrays(SampleInterval);
            // TODO : les couleurs sont attribuées selon l'ordre, décocher une série change la couleur des suivantes
            var line = PlotControl.Plot.Add.ScatterLine(xs, ys);
            var color = Avalonia.Media.Color.FromArgb(line.Color.A, line.Color.R, line.Color.G, line.Color.B);
            legend.Add(new LegendEntry(toggle.Name, new Avalonia.Media.SolidColorBrush(color)));
        }
        LegendList.ItemsSource = legend;

        var visiblePoints = ViewModel.Series
            .Where(t => t.IsChecked)
            .SelectMany(t => t.Series.Points)
            .ToList();

        // Rien n'empêche l'utilisateur de tout décocher
        PlotControl.Plot.Axes.Rules.Clear();
        if (visiblePoints.Count > 0)
        {
            _dataMinX = visiblePoints.Min(p => p.Timestamp).ToOADate();
            _dataMaxX = visiblePoints.Max(p => p.Timestamp).ToOADate();

            // L'axe Y est verrouillé sur toute la plage des valeurs
            var yMin = visiblePoints.Min(p => p.Value);
            var yMax = visiblePoints.Max(p => p.Value);
            var margin = (yMax - yMin) * 0.05;
            PlotControl.Plot.Axes.Rules.Add(new LockedVertical(PlotControl.Plot.Axes.Left, yMin - margin, yMax + margin));
        }

        // Affiche les dates lisibles sur l'axe du bas plutôt que les nombres bruts de ToOADate()
        PlotControl.Plot.Axes.DateTimeTicksBottom();
        // Force le contrôle à se redessiner à l'écran avec les nouvelles données
        PlotControl.Refresh();
        UpdateScrollBar();
    }

    private void UpdateScrollBar()
    {
        var limits = PlotControl.Plot.Axes.GetLimits();
        var visibleWidth = limits.Right - limits.Left;

        TimeScrollBar.Minimum = _dataMinX;
        TimeScrollBar.Maximum = Math.Max(_dataMinX, _dataMaxX - visibleWidth);
        TimeScrollBar.ViewportSize = visibleWidth;
        TimeScrollBar.Value = Math.Clamp(limits.Left, TimeScrollBar.Minimum, TimeScrollBar.Maximum);
    }

    private void OnTimeScroll(object? sender, ScrollEventArgs e)
    {
        var limits = PlotControl.Plot.Axes.GetLimits();
        var visibleWidth = limits.Right - limits.Left;

        PlotControl.Plot.Axes.SetLimitsX(e.NewValue, e.NewValue + visibleWidth);
        PlotControl.Refresh();
    }
}
