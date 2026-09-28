using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using SpaceWeather.Core.Models;

namespace SpaceWeather.App.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    // L'UI ne connait pas scottplot il se contante donc d'annoncer un changement
    public event EventHandler? PlotInvalidated;

    // Comme une List<T>, mais elle prévient automatiquement Avalonia quand il y a modification des checkboxes
    public ObservableCollection<SeriesToggleViewModel> Series { get; } = [];

    // Attribut mvvm qui génère une propriété publique HasData avec la notification de changement
    [ObservableProperty]
    private bool _hasData;

    // Imports
    public void LoadSeries(List<TimeSeries> series)
    {
        foreach (var toggle in Series)
            // Desabonnement aux anciennes données pour préparer un import
            toggle.PropertyChanged -= OnToggleChanged;

        // Vide la collection, comme series est une ObservableCollection, une mise a jour visuelle est directement déclanchée
        Series.Clear();

        foreach (var s in series)
        {
            // Pour chaque TimeSeries on crée un SeriesToggleViewModel
            var toggle = new SeriesToggleViewModel(s);
            // On s'abonne a son PropertyChanged
            toggle.PropertyChanged += OnToggleChanged;
            // Enfin on l'ajoute à Series
            Series.Add(toggle);
        }

        // On recalcule HasData
        HasData = Series.Count > 0;
        // On prévient que qqch a changé
        PlotInvalidated?.Invoke(this, EventArgs.Empty);
    }

    // Appelée quand l'utilisateur check/uncheck une donnée et declance le PropertyChanged
    private void OnToggleChanged(object? sender, PropertyChangedEventArgs e)
        => PlotInvalidated?.Invoke(this, EventArgs.Empty);
}
