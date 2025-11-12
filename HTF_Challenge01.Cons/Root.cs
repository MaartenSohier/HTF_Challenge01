using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HTF_Challenge01.Cons;

public class Root
{
    [JsonPropertyName("pressure")]
    public Pressure Pressure { get; set; }

    [JsonPropertyName("buoyancy")]
    public Buoyancy Buoyancy { get; set; }
}

public class Buoyancy
{
    [JsonPropertyName("submarineMass")]
    public double SubmarineMass { get; set; }

    [JsonPropertyName("centerOfMassOffset")]
    public double CenterOfMassOffset { get; set; }

    [JsonPropertyName("tankDistance")]
    public double TankDistance { get; set; }
}

public class Pressure
{
    [JsonPropertyName("depth")]
    public int Depth { get; set; }
}