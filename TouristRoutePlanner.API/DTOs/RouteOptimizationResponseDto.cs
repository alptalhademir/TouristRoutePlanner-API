using System.Text.Json.Serialization;
using TouristRoutePlanner.API.Models;

namespace TouristRoutePlanner.API.DTOs
{
    public class RouteOptimizationResponseDto
    {
        [JsonPropertyName("budget")]
        public decimal Budget { get; set; }
        [JsonPropertyName("fitness")]
        public double Fitness { get; set; }
        [JsonPropertyName("num_places")]
        public int NumPlaces { get; set; }
        [JsonPropertyName("score")]
        public double Score { get; set; }
        [JsonPropertyName("time")]
        public double Time { get; set; }
        [JsonPropertyName("visit_order")]
        public List<OptimizedPlace> VisitOrder { get; set; } = new();
    }
}
