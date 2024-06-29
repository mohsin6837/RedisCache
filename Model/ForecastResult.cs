namespace RedisCache.Model
{
    public class ForecastResult
    {
        public long ElapsedTime { get; set; }
        public IEnumerable<WeatherForecast>? Forecasts { get; set; }
        public ForecastResult(IEnumerable<WeatherForecast> forecasts,long elapsedTime) {
            ElapsedTime = elapsedTime;
            Forecasts = forecasts;
        }
    }
}
