using HTF_Challenge01.Cons;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

class Program
{
    static async Task Main(string[] args)
    {
        string baseUrl = "https://exs-htf-2025.azurewebsites.net";
        string teamKey = "97f4a7df-b18d-46fc-9f42-2b0193bbfabd";

        using HttpClient client = new();

        client.DefaultRequestHeaders.Authorization =
        new AuthenticationHeaderValue("Team", $"{teamKey}");

        var response = await client.GetAsync($"{baseUrl}/api/challenges/pressure?isTest=true");
        var jsonResponse = await response.Content.ReadAsStringAsync();

        var data = JsonSerializer.Deserialize<Root>(jsonResponse);

        double density = 1025; // kg/m³
        double gravity = 9.81; // m/s²
        double depth = data.Pressure.Depth;
        double pressure = density * gravity * depth;

        double submarineMass = data.Buoyancy.SubmarineMass;
        double centerOfMassOffset = data.Buoyancy.CenterOfMassOffset;
        double tankDistance = data.Buoyancy.TankDistance;

        double ballastMass = (submarineMass * centerOfMassOffset) / tankDistance;

        var answer = new
        {
            pressure = pressure,
            ballastMass = ballastMass
        };

        var jsonContent = new StringContent(
            JsonSerializer.Serialize(answer),
            Encoding.UTF8,
            "application/json"
        );

        var postResponse = await client.PostAsync($"{baseUrl}/api/challenges/pressure", jsonContent);
        var result = await postResponse.Content.ReadAsStringAsync();

        Console.WriteLine($"\nAPI Response: {result}");

    }
    
}
