using System.Globalization;
using System.Net.Http;
using System.Text.Json;
using TouristRoutePlanner.API.DTOs;
using TouristRoutePlanner.API.Models;
using TouristRoutePlanner.API.Repositories.Interfaces;
using TouristRoutePlanner.API.Services.Implementations;

namespace TouristRoutePlanner.API.Services.Interfaces
{
    public class RouteOptimizationService : IRouteOptimizationService
    {
        private readonly ITravelRepository travelRepository;
        private readonly IDistanceRepository distanceRepository;
        private readonly HttpClient httpClient;
        private readonly string optimizationApiUrl;

        public RouteOptimizationService(ITravelRepository travelRepository, IDistanceRepository distanceRepository, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            this.travelRepository = travelRepository;
            this.distanceRepository = distanceRepository;
            httpClient = httpClientFactory.CreateClient();
            optimizationApiUrl = configuration["OptimizationApi:Url"];
        }

        public async Task<List<RouteOptimizationResponseDto>> OptimizeRouteAsync(
        Guid travelId,
        Guid userId,
        OptimizationConstraints constraints)
        {
            // Get travel with places
            var travel = await travelRepository.GetByIdWithPlacesAsync(travelId, userId);
            if (travel == null)
                throw new KeyNotFoundException("Travel not found");

            // Prepare places for optimization
            var places = travel.TravelPlaces.Select((tp, index) => new PlaceForOptimization
            {
                Id = index,
                PlaceId = tp.Place.ExternalId,
                Latitude = tp.Place.Latitude,
                Longitude = tp.Place.Longitude,
                Name = tp.Place.DisplayName,
                Score = tp.Place.Rating * 2
            }).ToList();

            // Build time and cost matrices
            var numPlaces = places.Count;
            var timeMatrix = new double[numPlaces][];
            var costMatrix = new double[numPlaces][];

            for (int i = 0; i < numPlaces; i++)
            {
                timeMatrix[i] = new double[numPlaces];
                costMatrix[i] = new double[numPlaces];

                for (int j = 0; j < numPlaces; j++)
                {
                    // Initialize with default values
                    timeMatrix[i][j] = i == j ? 0 : int.MaxValue;
                    costMatrix[i][j] = i == j ? 0 : int.MaxValue;

                    if (i != j)
                    {
                        var distance = await distanceRepository.GetDistanceBetweenPlacesAsync(
                            places[i].PlaceId,
                            places[j].PlaceId);

                        if (distance != null)
                        {
                            // Convert string duration to minutes
                            timeMatrix[i][j] = ParseDuration(distance.WalkingDuration);
                            // Convert string distance to cost (simplified)
                            costMatrix[i][j] = ParseDistance(distance.WalkingDistance) / 100;
                        }
                    }
                }
            }

            // Create optimization request
            var request = new RouteOptimizationRequestDto
            {
                Places = places,
                TimeMatrix = timeMatrix,
                CostMatrix = costMatrix,
                Constraints = constraints
            };

            // Replace the problematic line
            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
                WriteIndented = true
            };

            // Send to optimization API
            var response = await httpClient.PostAsJsonAsync(optimizationApiUrl, request, jsonOptions);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<List<RouteOptimizationResponseDto>>();
        }

        private double ParseDuration(string duration)
        {
            // If the duration contains "mins", parse it as "X mins" format
            if (duration.Contains("mins"))
            {
                return int.Parse(duration.Split(' ')[0]);
            }
            // If it's a double value, it's already in minutes
            else if (double.TryParse(duration, out double value))
            {
                value = Convert.ToDouble(duration, CultureInfo.InvariantCulture);
                return value;
            }

            throw new FormatException($"Invalid duration format: {duration}");
        }

        private double ParseDistance(string distance)
        {
            // If the distance contains "m", parse it as "X m" format
            if (distance.Contains("m"))
            {
                return int.Parse(distance.Split(' ')[0]);
            }
            // If it's a double value, it's already in meters
            else if (double.TryParse(distance, out double value))
            {
                value = Convert.ToDouble(distance, CultureInfo.InvariantCulture);
                return value;
            }

            throw new FormatException($"Invalid distance format: {distance}");
        }
    }
}
