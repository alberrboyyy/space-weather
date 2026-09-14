using System.Text.Json;
using SpaceWeather.Core.Models;

namespace SpaceWeather.Core.Import;

public static class NoaaProtonFileImporter
{
    private const string Unit = "particles / (cm^2 s sr)";

    public static List<TimeSeries> ImportFromFile(string filePath)
        => ImportFromJson(File.ReadAllText(filePath));

    public static List<TimeSeries> ImportFromJson(string json)
    {
        var records = JsonSerializer.Deserialize<List<NoaaProtonRecord>>(json) ?? [];

        return records
            .GroupBy(r => r.Energy)
            .Select(group => new TimeSeries
            {
                Name = group.Key,
                Unit = Unit,
                Points = group
                    .Select(r => new TimeSeriesPoint(r.TimeTag, r.Flux))
                    .OrderBy(p => p.Timestamp)
                    .ToList()
            })
            .ToList();
    }
}
