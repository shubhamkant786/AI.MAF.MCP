using ModelContextProtocol.Server;
using System.ComponentModel;

namespace AI.MAF.MCP.Tools.Tools
{
    [McpServerToolType]
    public class LocationTool
    {
        [McpServerTool]
        [Description("Get the coordinates of a location.")]
        public static (double Latitude, double Longitude) GetCoordinates(string location)
        {
            // Placeholder implementation
            return (Latitude: 0.0, Longitude: 0.0);
        }
    }
}
