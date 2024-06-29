using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using StackExchange.Redis;
using System.Text.Json.Nodes;
using RedisCache.Model;
using System.Diagnostics;
using System.Text.Json;

namespace RedisCache.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly IDatabase _database;

        public WeatherForecastController(HttpClient httpClient, IConnectionMultiplexer connectionMultiplexer)
        {
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("weatherCachingApp", "1.0"));
            _database=connectionMultiplexer.GetDatabase();
        }

        [HttpGet(Name ="GetWeatherForecast")]
        public async Task<ForecastResult> Get([FromQuery] double latitude, [FromQuery] double longitude)
        {
            string? json;
            var watch=Stopwatch.StartNew();
            var keyName = $"forecast:{latitude},{longitude}";
            json = await _database.StringGetAsync(keyName);
            if (string.IsNullOrEmpty(json))
            {
                json = await GetForecast(latitude, longitude);
                var setTask=_database.StringSetAsync(keyName, json);
                var expireTask = _database.KeyExpireAsync(keyName, TimeSpan.FromSeconds(3600));
                await Task.WhenAll(setTask, expireTask);   
            }
            var forecast=JsonSerializer.Deserialize<IEnumerable<WeatherForecast>>(json);
            watch.Stop();
            var result=new ForecastResult(forecast,watch.ElapsedMilliseconds);
            return result;
        }
        private async Task<string> GetForecast(double latitude,double longitude)
        {
            var pointsRequestQuery = $"https://api.weather.gov/points/{latitude},{longitude}";
            var result=await _httpClient.GetFromJsonAsync<JsonObject>(pointsRequestQuery);
            var gridX = result["properties"]["gridX"].ToString();
            var gridY = result["properties"]["gridY"].ToString();
            var gridId = result["properties"]["gridId"].ToString();
            var forecastRequestQuery = $"https://api.weather.gov/gridpoints/{gridId}/{gridX},{gridY}";
            var forecastResult=await _httpClient.GetFromJsonAsync<JsonObject>(forecastRequestQuery);
            var periodsJson = forecastResult["properties"]["periods"].ToString();
            return periodsJson;
        }
    }
}
