using CommunityToolkit.Mvvm.ComponentModel;
using SpaceWeather.Core.Models;

namespace SpaceWeather.App.ViewModels;

public partial class SeriesToggleViewModel : ObservableObject
{
    public TimeSeries Series { get; }

    public string Name => Series.Name;

    [ObservableProperty]
    private bool _isChecked = true;

    public SeriesToggleViewModel(TimeSeries series)
    {
        Series = series;
    }
}
