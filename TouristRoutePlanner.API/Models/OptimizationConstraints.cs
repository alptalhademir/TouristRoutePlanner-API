using System.Text.Json.Serialization;

namespace TouristRoutePlanner.API.Models
{
    public class OptimizationConstraints
    {
        [JsonPropertyName("max_budget")]
        public decimal MaxBudget { get; set; }
        [JsonPropertyName("max_time")]
        public int MaxTime { get; set; }
        [JsonPropertyName("max_places_to_visit")]
        public int MaxPlacesToVisit { get; set; }
    }
}
