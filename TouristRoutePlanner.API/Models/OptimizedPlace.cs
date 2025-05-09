using System.Text.Json.Serialization;

namespace TouristRoutePlanner.API.Models
{
    public class OptimizedPlace
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("place_id")]
        public string PlaceId { get; set; }
        [JsonPropertyName("score")]
        public double Score { get; set; }
    }
}
