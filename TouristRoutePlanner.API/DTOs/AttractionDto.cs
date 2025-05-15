using System.Text.Json.Serialization;

namespace TouristRoutePlanner.API.DTOs
{
    public class AttractionDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("score")]
        public double Score { get; set; }

        [JsonPropertyName("budget")]
        public double Budget { get; set; }

        [JsonPropertyName("time")]
        public double Time { get; set; }

        [JsonPropertyName("location")]
        public double[] Location { get; set; }

        [JsonPropertyName("category")]
        public string Category { get; set; }
    }
}
