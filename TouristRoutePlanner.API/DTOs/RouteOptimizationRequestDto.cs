using System.Text.Json.Serialization;
using TouristRoutePlanner.API.Models;

namespace TouristRoutePlanner.API.DTOs
{
    public class RouteOptimizationRequestDto
    {
        [JsonPropertyName("places")]
        public List<PlaceForOptimization> Places { get; set; } = new();
        [JsonPropertyName("time_matrix")]
        public double[][] TimeMatrix { get; set; }
        [JsonPropertyName("cost_matrix")]
        public double[][] CostMatrix { get; set; }
        [JsonPropertyName("constraints")]
        public OptimizationConstraints Constraints { get; set; }
    }
}
