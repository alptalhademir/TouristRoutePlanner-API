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

        public async Task<List<PathGenerationResponseDto>> GeneratePathsAsync(Guid travelId,
            Guid userId, PathGenerationRequestDto request)
        {
            // Get travel with places
            var travel = await travelRepository.GetByIdWithPlacesAsync(travelId, userId);
            if (travel == null)
                throw new KeyNotFoundException("Travel not found");

            // If attractions not specified, convert places from travel
            if (request.Attractions == null || !request.Attractions.Any())
            {
                request.Attractions = travel.TravelPlaces.Select((tp, index) => new AttractionDto
                {
                    Id = index,
                    Name = tp.Place.DisplayName,
                    Score = tp.Place.Rating,
                    Budget = CalculateEstimatedBudget(tp.Place),
                    Time = EstimateVisitTime(tp.Place),
                    Location = new double[] { tp.Place.Latitude, tp.Place.Longitude },
                    Category = tp.Place.PlaceTypes?.FirstOrDefault(pt => pt.Type?.Name != null)?.Type.Name ?? "unknown"
                }).ToList();
            }

            if(request.MaxAttractions > request.Attractions.Count)
            {
                request.MaxAttractions = request.Attractions.Count;
            }

            // If user location not specified, use the first place's location
            if (request.UserLocation == null || request.UserLocation.Length != 2)
            {
                var firstPlace = travel.TravelPlaces.FirstOrDefault()?.Place;

                if (firstPlace != null)
                {
                    request.UserLocation = new double[] { firstPlace.Latitude, firstPlace.Longitude };
                }

                else
                {
                    request.UserLocation = new double[] { 41.0082, 28.9784 };
                }
            }

            // If constraints not specified, create default ones
            if (request.Constraints == null || !request.Constraints.Any())
            {
                request.Constraints = new Dictionary<string, ConstraintDto>
                {
                    ["budget"] = new ConstraintDto
                    {
                        Name = "budget",
                        CurrentValue = 0.0,
                        MaxValue = 700.0,
                        PenaltyWeight = 1.0
                    },
                    ["time"] = new ConstraintDto
                    {
                        Name = "time",
                        CurrentValue = 0.0,
                        MaxValue = 480.0,
                        PenaltyWeight = 1.0
                    },
                    ["category_diversity"] = new ConstraintDto
                    {
                        Name = "category_diversity",
                        CurrentValue = new List<string>(),
                        PenaltyWeight = 0.5
                    }
                };
            }

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            var response = await httpClient.PostAsJsonAsync(
                $"{optimizationApiUrl}/generate-paths",
                request,
                jsonOptions);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<List<PathGenerationResponseDto>>();
        }

        private double CalculateEstimatedBudget(Place place)
        {
            return place.PriceLevel switch
            {
                "PRICE_LEVEL_FREE" => 0,
                "PRICE_LEVEL_INEXPENSIVE" => 10,
                "PRICE_LEVEL_MODERATE" => 25,
                "PRICE_LEVEL_EXPENSIVE" => 50,
                "PRICE_LEVEL_VERY_EXPENSIVE" => 100,
                _ => 20
            };
        }

        private double EstimateVisitTime(Place place)
        {
            var type = place.PlaceTypes.FirstOrDefault()?.Type.Name;
            return type switch
            {
                "museum" => 120, // 2 hours
                "park" => 60,    // 1 hour
                "restaurant" => 90, // 1.5 hours
                _ => 60 // default 1 hour
            };
        }

    }
}
