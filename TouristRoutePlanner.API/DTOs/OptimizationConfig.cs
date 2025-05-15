using System.Text.Json.Serialization;

namespace TouristRoutePlanner.API.DTOs
{
    public class OptimizationConfig
    {
        [JsonPropertyName("user_location")]
        public double[] UserLocation { get; set; }

        [JsonPropertyName("max_budget")]
        public double MaxBudget { get; set; }

        [JsonPropertyName("max_time")]
        public double MaxTime { get; set; }

        [JsonPropertyName("required_category")]
        public string RequiredCategory { get; set; }

        [JsonPropertyName("max_attractions")]
        public int MaxAttractions { get; set; }
    }
}
