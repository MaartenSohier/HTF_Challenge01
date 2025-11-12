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
        client.DefaultRequestHeaders.Add("Authorization", $"Team {teamKey}");

        // Get the data
        var response = await client.GetAsync($"{baseUrl}/api/challenges/reparations?isTest=false");
        
        Console.WriteLine($"GET Status Code: {response.StatusCode}");
        
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"ERROR: GET request failed!");
            Console.WriteLine($"Status: {response.StatusCode}");
            Console.WriteLine($"Response: {errorContent}");
            return;
        }
        
        var jsonResponse = await response.Content.ReadAsStringAsync();

        Console.WriteLine("Response from API:");
        Console.WriteLine(jsonResponse);

        // Deserialize
        var data = JsonSerializer.Deserialize<Root>(jsonResponse);

        Console.WriteLine($"\nDepth: {data?.Pressure?.Depth}");
        Console.WriteLine($"Submarine Mass: {data?.Buoyancy?.SubmarineMass}");
        Console.WriteLine($"Center of Mass Offset: {data?.Buoyancy?.CenterOfMassOffset}");
        Console.WriteLine($"Tank Distance: {data?.Buoyancy?.TankDistance}");

        if (data == null || data.Pressure == null || data.Buoyancy == null)
        {
            Console.WriteLine("ERROR: Deserialization failed!");
            return;
        }

        // 1. Calculate absolute pressure
        const double atmosphericPressure = 101325.0; // Pa
        const double seawaterDensity = 1025.0; // kg/m³
        const double gravity = 9.81; // m/s²
        double depth = data.Pressure.Depth;
        double pressure = atmosphericPressure + (seawaterDensity * gravity * depth);
        pressure = Math.Round(pressure, 2);

        Console.WriteLine($"\nCalculated Absolute Pressure: {pressure} Pa");

        // 2. Calculate ballast tank volumes
        
        double d = data.Buoyancy.TankDistance;
        double M = data.Buoyancy.SubmarineMass;
        double offset = data.Buoyancy.CenterOfMassOffset;
        const double waterDensity = 1025.0; // kg/m³ 

        double massDifference = -M * offset / d;
        
        double m1 = M / 3.0 + M * offset / (2.0 * d);
        double m3 = m1 - M * offset / d;
        double m2 = M - m1 - m3;

        double v1 = m1 / waterDensity;
        double v2 = m2 / waterDensity;
        double v3 = m3 / waterDensity;

        v1 = Math.Round(v1, 2);
        v2 = Math.Round(v2, 2);
        v3 = Math.Round(v3, 2);

        Console.WriteLine($"\nCalculated Tank Volumes:");
        Console.WriteLine($"Front tank (at -{d}m): {v1} m³");
        Console.WriteLine($"Middle tank (at 0m): {v2} m³");
        Console.WriteLine($"Rear tank (at {d}m): {v3} m³");

        var answer = new
        {
            answer = new
            {
                pressure = pressure,
                tankVolumes = new[] { v1, v2, v3 }
            }
        };

        Console.WriteLine("\nSending this JSON:");
        var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
        Console.WriteLine(JsonSerializer.Serialize(answer, jsonOptions));

        var jsonContent = new StringContent(
            JsonSerializer.Serialize(answer),
            Encoding.UTF8,
            "application/json"
        );

        var postResponse = await client.PostAsync($"{baseUrl}/api/challenges/reparations", jsonContent);
        
        Console.WriteLine($"\nPOST Status Code: {postResponse.StatusCode}");
        
        var result = await postResponse.Content.ReadAsStringAsync();

        if (!postResponse.IsSuccessStatusCode)
        {
            Console.WriteLine($"ERROR: POST request failed!");
            Console.WriteLine($"Status: {postResponse.StatusCode}");
        }
        
        Console.WriteLine($"API Response: {result}");
    }
}
