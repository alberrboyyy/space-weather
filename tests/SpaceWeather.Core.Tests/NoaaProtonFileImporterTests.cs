using SpaceWeather.Core.Import;

namespace SpaceWeather.Core.Tests;

public class NoaaProtonFileImporterTests
{
    [Fact]
    public void ImportFromJson_TwoEnergies_CreatesOneSortedSeriesPerEnergy()
    {
        const string json = """
            [
              { "time_tag": "2026-10-05T10:05:00Z", "satellite": 19, "flux": 4.0, "energy": ">=10 MeV" },
              { "time_tag": "2026-10-05T10:00:00Z", "satellite": 19, "flux": 1.0, "energy": ">=1 MeV" },
              { "time_tag": "2026-10-05T10:00:00Z", "satellite": 19, "flux": 3.0, "energy": ">=10 MeV" },
              { "time_tag": "2026-10-05T10:05:00Z", "satellite": 19, "flux": 2.0, "energy": ">=1 MeV" }
            ]
            """;

        var series = NoaaProtonFileImporter.ImportFromJson(json);

        Assert.Equal(2, series.Count);
        var low = Assert.Single(series, s => s.Name == ">=1 MeV");
        var high = Assert.Single(series, s => s.Name == ">=10 MeV");
        Assert.Equal(new[] { 1.0, 2.0 }, low.Points.Select(p => p.Value));
        Assert.Equal(new[] { 3.0, 4.0 }, high.Points.Select(p => p.Value));
    }
}
