using SpaceWeather.Core.Models;
using SpaceWeather.Core.Storage;

namespace SpaceWeather.Core.Tests;

public class LocalSeriesStoreTests
{
    [Fact]
    public void Merge_DuplicateTimestamp_KeepsImportedValueAndSorts()
    {
        var t0 = new DateTime(2026, 10, 5, 10, 0, 0);
        List<TimeSeries> current =
        [
            new TimeSeries
            {
                Name = ">=10 MeV",
                Unit = "pfu",
                Points = [new(t0, 1), new(t0.AddMinutes(5), 2)]
            }
        ];
        List<TimeSeries> imported =
        [
            new TimeSeries
            {
                Name = ">=10 MeV",
                Unit = "pfu",
                Points = [new(t0.AddMinutes(10), 3), new(t0.AddMinutes(5), 20)]
            }
        ];

        var merged = LocalSeriesStore.Merge(current, imported);

        var points = Assert.Single(merged).Points;
        Assert.Equal(new[] { t0, t0.AddMinutes(5), t0.AddMinutes(10) }, points.Select(p => p.Timestamp));
        Assert.Equal(new[] { 1.0, 20.0, 3.0 }, points.Select(p => p.Value));
    }
}
