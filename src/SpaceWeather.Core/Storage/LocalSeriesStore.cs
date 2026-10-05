using System.Text.Json;
using SpaceWeather.Core.Models;

namespace SpaceWeather.Core.Storage;

public static class LocalSeriesStore
{
    // Liste vide si le fichier n'existe pas encore
    public static List<TimeSeries> Load(string filePath)
    {
        if (!File.Exists(filePath))
            return [];

        var json = File.ReadAllText(filePath);
        return JsonSerializer.Deserialize<List<TimeSeries>>(json) ?? [];
    }

    public static void Save(string filePath, List<TimeSeries> series)
    {
        // Crée le dossier parent s'il n'existe pas encore
        var directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directory))
            Directory.CreateDirectory(directory);

        var json = JsonSerializer.Serialize(series);
        File.WriteAllText(filePath, json);
    }

    // Fusion des series deja stockées et de l'import
    public static List<TimeSeries> Merge(List<TimeSeries> current, List<TimeSeries> imported)
    {
        return current
            .Concat(imported)
            .GroupBy(s => s.Name)
            .Select(group => new TimeSeries
            {
                Name = group.Key,
                Unit = group.First().Unit,
                Points = group
                    .SelectMany(s => s.Points)
                    .GroupBy(p => p.Timestamp)
                    .Select(g => g.Last())
                    .OrderBy(p => p.Timestamp)
                    .ToList()
            })
            .ToList();
    }
}
