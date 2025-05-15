using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace TouristRoutePlanner.API.Models
{
    public class RefreshToken
    {
        public Guid Id { get; set; }
        public string Token { get; set; }
        public Guid UserId { get; set; }
        public DateTime ExpiryTime { get; set; }
        public int UsedCount { get; set; } = 0;
    }
}
