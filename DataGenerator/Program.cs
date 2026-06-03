using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

const string DefaultTargetUrl = "http://middleware-api:8080/api/logs";
var targetUrl = Environment.GetEnvironmentVariable("TARGET_URL") ?? DefaultTargetUrl;
using var httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };
var random = Random.Shared;

string RandomAlphanumeric(int length)
{
    const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    return new string(Enumerable.Range(0, length).Select(_ => chars[random.Next(chars.Length)]).ToArray());
}

string RandomDigits(int length)
{
    return new string(Enumerable.Range(0, length).Select(_ => (char)('0' + random.Next(10))).ToArray());
}

string RandomEmail()
{
    var local = RandomAlphanumeric(8).ToLowerInvariant();
    return $"{local}@example.com";
}

string RandomSymbol() => new[] { "AAPL", "THYAO", "MSFT", "GOOGL", "AMZN", "TSLA" }[random.Next(6)];
string RandomAction() => random.Next(2) == 0 ? "BUY" : "SELL";
string RandomLogLevel() => new[] { "INFO", "WARNING", "DEBUG", "CRITICAL" }[random.Next(4)];

decimal RandomAmount() => Math.Round((decimal)(random.NextDouble() * 9999.0 + 1.0), 2);

Console.WriteLine($"Starting DataGenerator, sending logs to {targetUrl}");

while (true)
{
    var payload = new
    {
        timestamp = DateTime.UtcNow.ToString("o"),
        sender_id = Guid.NewGuid().ToString(),
        transaction_no = RandomAlphanumeric(12),
        sensitive_data = new
        {
            tc_kimlik = RandomDigits(11),
            credit_card = RandomDigits(16),
            email = RandomEmail()
        },
        transaction_details = new
        {
            symbol = RandomSymbol(),
            action = RandomAction(),
            amount = RandomAmount()
        },
        log_level = RandomLogLevel()
    };

    try
    {
        var response = await httpClient.PostAsJsonAsync(targetUrl, payload);
        Console.WriteLine(response.IsSuccessStatusCode
            ? $"SUCCESS: {response.StatusCode}"
            : $"FAIL: {response.StatusCode} - {response.ReasonPhrase}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"FAIL: {ex.Message}");
    }

    await Task.Delay(50);
}
