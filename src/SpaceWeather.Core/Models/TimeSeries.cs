namespace SpaceWeather.Core.Models;

public class TimeSeries
{
    public required string Name { get; init; }
    public required string Unit { get; init; }
    public List<TimeSeriesPoint> Points { get; init; } = [];
}
