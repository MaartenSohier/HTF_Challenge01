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

        var response = await client.GetAsync($"{baseUrl}/api/challenges/signal?isTest=true");
        var jsonResponse = await response.Content.ReadAsStringAsync();

        Console.WriteLine("Response:");
        Console.WriteLine(jsonResponse);

        var signalData = JsonSerializer.Deserialize<SignalResponse>(jsonResponse);

        string encryptedMessage = signalData?.CipherText ?? "";
        int shift = signalData?.Shift ?? 0;

        Console.WriteLine($"\nEncrypted: {encryptedMessage}");
        Console.WriteLine($"Shift: {shift}");

        // Step 3: Decrypt the message
        string decryptedMessage = DecryptCaesar(encryptedMessage, shift);

        Console.WriteLine($"Decrypted: {decryptedMessage}");

        // Step 4: POST the answer back
        Console.WriteLine("\nSending answer...");

        var answerData = new { answer = decryptedMessage };
        var jsonContent = new StringContent(
            JsonSerializer.Serialize(answerData),
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
