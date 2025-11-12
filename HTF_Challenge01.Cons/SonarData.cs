using System.Text.Json.Serialization;

namespace HTF_Challenge01.Cons;

public class SonarResponse
{
    [JsonPropertyName("startPoint")]
    public double[] StartPoint { get; set; } = [];

    [JsonPropertyName("endPoint")]
    public double[] EndPoint { get; set; } = [];

    [JsonPropertyName("sonarHeatmap")]
    public int[][][] SonarHeatmap { get; set; } = [];

    [JsonPropertyName("fuelCount")]
    public int FuelCount { get; set; }
}

