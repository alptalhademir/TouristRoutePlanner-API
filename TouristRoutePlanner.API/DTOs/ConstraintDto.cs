using System.Text.Json.Serialization;

namespace TouristRoutePlanner.API.DTOs
{
    public class ConstraintDto
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("current_value")]
        public object CurrentValue { get; set; }

        [JsonPropertyName("max_value")]
        public double? MaxValue { get; set; }

        [JsonPropertyName("penalty_weight")]
        public double PenaltyWeight { get; set; }
    }
}
