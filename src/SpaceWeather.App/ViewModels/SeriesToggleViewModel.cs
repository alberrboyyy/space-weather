using CommunityToolkit.Mvvm.ComponentModel;
using SpaceWeather.Core.Models;

namespace SpaceWeather.App.ViewModels;

// Encapsulation d'une TimeSeries avec un booléen check/uncheck et Name qui est un raccourci vers Series.Name
// pour que le xaml n'aie pas a connaitre la structure interne de TimeSeries mais puisse afficher son nom
public partial class SeriesToggleViewModel : ObservableObject
{
    public TimeSeries Series { get; }

    public string Name => Series.Name;

    [ObservableProperty]
    private bool _isChecked = true;

    // Constructeur : stockage de la ref de SeriesToggleViewModel vers sa TimeSeries
    public SeriesToggleViewModel(TimeSeries series)
    {
        Series = series;
    }
}
