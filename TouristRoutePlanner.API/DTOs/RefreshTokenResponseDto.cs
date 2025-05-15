namespace TouristRoutePlanner.API.DTOs
{
    public class RefreshTokenResponseDto
    {
        public string JwtToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
