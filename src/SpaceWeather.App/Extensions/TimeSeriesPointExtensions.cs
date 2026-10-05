using System;
using System.Collections.Generic;
using System.Linq;
using SpaceWeather.Core.Models;

namespace SpaceWeather.App.Extensions;

public static class TimeSeriesPointExtensions
{
    // Quand l'écart avec le point précédent dépasse breakThreshold alors on
    // coupe la liaison entre les points pour eviter des liaisons trompeuses
    public static (double[] Xs, double[] Ys) ToPlotArrays(this List<TimeSeriesPoint> points, TimeSpan breakThreshold)
    {
        if (points.Count == 0)
            return ([], []);

        var entries = points
            // Associe chaque point à son précédent
            .Zip(points.Skip(1), (prev, curr) => (prev, curr))
            // Pour chaque paire, 1 ou 2 entrées selon si il y a un trou ou non
            .SelectMany(pair => ToPlotEntries(pair.prev, pair.curr, breakThreshold))
            // Ajout du premier point après le skip de celui ci
            .Prepend((X: points[0].Timestamp.ToOADate(), Y: points[0].Value))
            .ToList();

        return (entries.Select(e => e.X).ToArray(), entries.Select(e => e.Y).ToArray());
    }

    private static IEnumerable<(double X, double Y)> ToPlotEntries(TimeSeriesPoint prev, TimeSeriesPoint curr, TimeSpan breakThreshold)
    {
        if (curr.Timestamp - prev.Timestamp > breakThreshold)
            // yield est un raccourci compilateur pour éviter à avoir a retourner une liste temporaire complète
            yield return (curr.Timestamp.ToOADate(), double.NaN);

        // Même chose
        yield return (curr.Timestamp.ToOADate(), curr.Value);
    }
}
