using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

public class WeatherService : IWeatherService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;
    private readonly IMemoryCache _cache;

    public WeatherService(HttpClient httpClient, IConfiguration config, IMemoryCache cache)
    {
        _httpClient = httpClient;
        _config = config;
        _cache = cache;
    }

    public async Task<string> GetWeatherAsync(string city)
    {
        string cacheKey = $"weather_{city}";

        var watch = System.Diagnostics.Stopwatch.StartNew();


        if (_cache.TryGetValue(cacheKey, out string? cachedData))
        {
            Console.WriteLine("CACHE HIT");
            return cachedData!;
        }

        Console.WriteLine("CACHE MISS - calling external API");

        var apiKey = Environment.GetEnvironmentVariable("WEATHER_API_KEY");
        var baseUrl = _config["WeatherApi:BaseUrl"];
        
        Console.WriteLine(apiKey); // Debugging line to check if API key is loaded
        Console.WriteLine(baseUrl); // Debugging line to check if Base URL is loaded
        
        
        var url = $"{baseUrl}?q={city}&appid={apiKey}&units=metric";

        var response = await _httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Error fetching weather data");
        }

        var content = await response.Content.ReadAsStringAsync();

        // Cache for 5 minutes
        _cache.Set(cacheKey, content, TimeSpan.FromMinutes(5));
        
        watch.Stop();
        Console.WriteLine($"API call duration: {watch.ElapsedMilliseconds} ms");
        
        return content;
    }
}