using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

class Program
{
    static async Task Main(string[] args)
    {
        string baseUrl = "https://exs-htf-2025.azurewebsites.net";
        string teamKey = "97f4a7df-b18d-46fc-9f42-2b0193bbfabd"; 

        using HttpClient client = new HttpClient();
        client.DefaultRequestHeaders.Add("Authorization", $"Team {teamKey}");

        // Step 1: Get the encrypted signal
        var response = await client.GetAsync($"{baseUrl}/api/challenges/signal?isTest=true");
        var jsonResponse = await response.Content.ReadAsStringAsync();

        Console.WriteLine("Encrypted signal received:");
        Console.WriteLine(jsonResponse);

        // Parse the response to get the encrypted message
        var signalData = JsonSerializer.Deserialize<SignalResponse>(jsonResponse);
        string encryptedMessage = signalData?.Signal ?? "";

        Console.WriteLine($"\nEncrypted: {encryptedMessage}");

        // Step 2: Decrypt using Caesar cipher
        string decryptedMessage = DecryptCaesar(encryptedMessage);

        Console.WriteLine($"Decrypted: {decryptedMessage}");

        // Step 3: POST the answer back
        var answerData = new { answer = decryptedMessage };
        var jsonContent = new StringContent(
            JsonSerializer.Serialize(answerData),
            Encoding.UTF8,
            "application/json"
        );

        var postResponse = await client.PostAsync($"{baseUrl}/api/challenges/signal", jsonContent);
        var result = await postResponse.Content.ReadAsStringAsync();

        Console.WriteLine($"\nAPI Response: {result}");
    }

    static string DecryptCaesar(string encrypted)
    {
        // Try all possible shifts (1-25)
        for (int shift = 1; shift <= 25; shift++)
        {
            string decrypted = ShiftText(encrypted, -shift);
            Console.WriteLine($"Shift {shift}: {decrypted}");

            // You can manually check which one makes sense
            // Or implement logic to detect Dutch words
        }

        // Return the one that makes sense (you'll need to identify it)
        // For now, try shift 3 (common for Caesar cipher)
        return ShiftText(encrypted, -3);
    }

    static string ShiftText(string text, int shift)
    {
        StringBuilder result = new StringBuilder();

        foreach (char c in text)
        {
            if (char.IsLetter(c))
            {
                char basis = char.IsUpper(c) ? 'A' : 'a';
                int offset = c - basis;
                offset = (offset + shift + 26) % 26;
                result.Append((char)(basis + offset));
            }
            else
            {
                result.Append(c);
            }
        }

        return result.ToString();
    }
}

class SignalResponse
{
    public string Signal { get; set; }
}