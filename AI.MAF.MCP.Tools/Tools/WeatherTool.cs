using ModelContextProtocol.Server;
using System.ComponentModel;

namespace AI.MAF.MCP.Tools.Tools
{
    [McpServerToolType]
    public class WeatherTool
    {
        //[McpServerTool]
        //[Description("Get average of two temperatures and returns the result.")]
        //public static double Add(double a, double b)
        //{
        //    return (a + b) / 2;
        //}

        [McpServerTool(Name ="get_weather_forecast")]
        [Description("Gets the weather forecast for the specified city.")]
        public static string GetWeatherForecast(string city)
        {
            // Placeholder implementation
            return $"The weather forecast for {city} is sunny.";
        }
    }
}
