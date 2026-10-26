using SpaceWeather.Core.Extensions;
using SpaceWeather.Core.Models;

namespace SpaceWeather.Core.Tests;

public class TimeSeriesPointExtensionsTests
{
    [Fact]
    public void ToPlotArrays_GapInData_InsertsNaNBeforeNextPoint()
    {
        var t0 = new DateTime(2026, 10, 5, 10, 0, 0);
        List<TimeSeriesPoint> points = [new(t0, 1), new(t0.AddMinutes(5), 2), new(t0.AddMinutes(30), 3)];

        var (xs, ys) = points.ToPlotArrays(TimeSpan.FromMinutes(5));

        Assert.Equal(new[] { 1.0, 2.0, double.NaN, 3.0 }, ys);
        Assert.Equal(t0.AddMinutes(30).ToOADate(), xs[2]);
    }
}
