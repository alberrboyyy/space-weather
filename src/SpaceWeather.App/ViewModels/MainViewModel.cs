using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using SpaceWeather.Core.Models;

namespace SpaceWeather.App.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    public event EventHandler? PlotInvalidated;

    public ObservableCollection<SeriesToggleViewModel> Series { get; } = [];

    [ObservableProperty]
    private bool _hasData;

    public void LoadSeries(List<TimeSeries> series)
    {
        foreach (var toggle in Series)
            toggle.PropertyChanged -= OnToggleChanged;

        Series.Clear();

        foreach (var s in series)
        {
            var toggle = new SeriesToggleViewModel(s);
            toggle.PropertyChanged += OnToggleChanged;
            Series.Add(toggle);
        }

        HasData = Series.Count > 0;
        PlotInvalidated?.Invoke(this, EventArgs.Empty);
    }

    private void OnToggleChanged(object? sender, PropertyChangedEventArgs e)
        => PlotInvalidated?.Invoke(this, EventArgs.Empty);
}
