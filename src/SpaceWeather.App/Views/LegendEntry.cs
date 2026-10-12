using Avalonia.Media;

namespace SpaceWeather.App.Views;

// Une ligne de la légende : nom de la série et couleur que ScottPlot lui a donnée
public record LegendEntry(string Name, IBrush Brush);
