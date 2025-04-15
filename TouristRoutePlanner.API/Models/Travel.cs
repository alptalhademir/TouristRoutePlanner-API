namespace TouristRoutePlanner.API.Models
{
    public enum TravelerType
    {
        JustMe,
        Couple,
        Family,
        Friends
    }

    public class Travel
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public string City { get; set; }
        public string Title { get; set; } = "My Travel";

        public Guid TravelerTypeId { get; set; }
        public TravelerType TravelerType { get; set; }

        public ICollection<TravelType> TravelTypes { get; set; }
            = new List<TravelType>();

        public ICollection<TravelPlace> TravelPlaces { get; set; }
            = new List<TravelPlace>();
    }
}
