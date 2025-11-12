using System.Text.Json.Serialization;

namespace HTF_Challenge01.Cons;

public class Buoyancy
{
    [JsonPropertyName("submarineMass")]
    public double SubmarineMass { get; set; }

    [JsonPropertyName("centerOfMassOffset")]
    public double CenterOfMassOffset { get; set; }

    [JsonPropertyName("tankDistance")]
    public double TankDistance { get; set; }
}
