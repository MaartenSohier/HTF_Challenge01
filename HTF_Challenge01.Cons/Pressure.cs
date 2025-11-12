using System.Text.Json.Serialization;

namespace HTF_Challenge01.Cons;

public class Pressure
{
    [JsonPropertyName("depth")]
    public int Depth { get; set; }
}
