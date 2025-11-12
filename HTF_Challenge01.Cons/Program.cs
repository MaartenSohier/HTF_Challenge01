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

        string decryptedMessage = DecryptCaesar(encryptedMessage, shift);

        Console.WriteLine($"Decrypted: {decryptedMessage}");

        //POST
        var answerData = new { answer = decryptedMessage };
        var jsonContent = new StringContent(
            JsonSerializer.Serialize(answerData),
            Encoding.UTF8,
            "application/json"
        );

        var postResponse = await client.PostAsync($"{baseUrl}/api/challenges/signal", jsonContent);
        var result = await postResponse.Content.ReadAsStringAsync();

        Console.WriteLine($"API Response: {result}");
    }

    static string DecryptCaesar(string text, int shift)
    {
        string result = "";

        foreach (char c in text)
        {
            if (char.IsLetter(c))
            {
                // Get the base (A for uppercase, a for lowercase)
                char baseChar = char.IsUpper(c) ? 'A' : 'a';

                // Shift the character (subtract shift to decrypt)
                int newPosition = (c - baseChar - shift + 26) % 26;
                result += (char)(baseChar + newPosition);
            }
            else
            {
                // Keep spaces, punctuation, etc. as-is
                result += c;
            }
        }

        return result;
    }
}
