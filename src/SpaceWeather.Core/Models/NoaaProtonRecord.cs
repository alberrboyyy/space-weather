using System.Text.Json.Serialization;

namespace SpaceWeather.Core.Models;

public class NoaaProtonRecord
{
    [JsonPropertyName("time_tag")]
    public DateTime TimeTag { get; init; }

    [JsonPropertyName("satellite")]
    public int Satellite { get; init; }

    [JsonPropertyName("flux")]
    public double Flux { get; init; }

    [JsonPropertyName("energy")]
    public string Energy { get; init; } = string.Empty;
}
