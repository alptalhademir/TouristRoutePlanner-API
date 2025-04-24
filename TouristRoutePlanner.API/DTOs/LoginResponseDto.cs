namespace TouristRoutePlanner.API.DTOs
{
    public class LoginResponseDto
    {
        public string jwtToken { get; set; }
        public UserDto User { get; set; }
    }
}
