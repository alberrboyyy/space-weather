using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using SpaceWeather.Core.Models;

namespace SpaceWeather.App.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    // L'UI ne connait pas scottplot il se contante donc d'annoncer un changement
    public event EventHandler? PlotInvalidated;

    // Comme une List<T>, mais elle prévient automatiquement Avalonia quand il y a modification des checkboxes.
    // ObservableProperty pour que l'UI soit aussi prévenue quand LoadSeries remplace la collection entière
    [ObservableProperty]
    private ObservableCollection<SeriesToggleViewModel> _series = [];

    // Attribut mvvm qui génère une propriété publique HasData avec la notification de changement
    [ObservableProperty]
    private bool _hasData;

    // Message affiché en cas d'échec d'un import ou d'une récupération NOAA
    [ObservableProperty]
    private string? _errorMessage;

    // Imports
    // TODO : les cases décochées sont recochées après chaque import, l'état IsChecked n'est pas conservé
    public void LoadSeries(List<TimeSeries> series)
    {
        foreach (var toggle in Series)
            // Desabonnement aux anciennes données pour préparer un import
            toggle.PropertyChanged -= OnToggleChanged;

        // Pour chaque TimeSeries on crée un SeriesToggleViewModel, puis on remplace l'ancienne collection
        Series = new ObservableCollection<SeriesToggleViewModel>(series.Select(s => new SeriesToggleViewModel(s)));

        foreach (var toggle in Series)
            // On s'abonne a son PropertyChanged
            toggle.PropertyChanged += OnToggleChanged;

        // On recalcule HasData
        HasData = Series.Count > 0;
        // On prévient que qqch a changé
        PlotInvalidated?.Invoke(this, EventArgs.Empty);
    }

    // Appelée quand l'utilisateur check/uncheck une donnée et declance le PropertyChanged
    private void OnToggleChanged(object? sender, PropertyChangedEventArgs e)
        => PlotInvalidated?.Invoke(this, EventArgs.Empty);
}
