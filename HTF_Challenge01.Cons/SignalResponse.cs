using System.Text.Json.Serialization;

class SignalResponse
{
    [JsonPropertyName("cipherText")]
    public string CipherText { get; set; }

    [JsonPropertyName("shift")]
    public int Shift { get; set; }
}