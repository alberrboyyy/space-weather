using System;
using System.IO;

namespace SpaceWeather.App;

public static class AppPaths
{
    // ApplicationData est le fichier de stockage de données applications sur chaque os
    public static readonly string SeriesFile = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "SpaceWeather",
        "series.json");
}
