using System.Text.Json.Serialization;

namespace TouristRoutePlanner.API.DTOs
{
    public class PathGenerationRequestDto
    {
        [JsonPropertyName("attractions")]
        public List<AttractionDto>? Attractions { get; set; }

        [JsonPropertyName("user_location")]
        public double[] UserLocation { get; set; }

        [JsonPropertyName("constraints")]
        public Dictionary<string, ConstraintDto> Constraints { get; set; }

        [JsonPropertyName("max_attractions")]
        public int MaxAttractions { get; set; }

        [JsonPropertyName("mode")]
        public string Mode { get; set; } = "walking";
    }
}
