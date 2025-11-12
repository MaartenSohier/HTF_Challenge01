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