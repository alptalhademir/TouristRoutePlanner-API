﻿using System.Text.Json.Serialization;

namespace TouristRoutePlanner.API.DTOs
{
    public class PathGenerationResponseDto
    {
        [JsonPropertyName("attractions")]
        public List<AttractionDto> Attractions { get; set; }

        [JsonPropertyName("score")]
        public double Score { get; set; }

        [JsonPropertyName("budget")]
        public double Budget { get; set; }

        [JsonPropertyName("time")]
        public double Time { get; set; }

        [JsonPropertyName("fitness")]
        public double Fitness { get; set; }

        [JsonPropertyName("violations")]
        public double Violations { get; set; }
    }
}
