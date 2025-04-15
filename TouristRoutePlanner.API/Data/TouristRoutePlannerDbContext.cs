using Microsoft.EntityFrameworkCore;
using TouristRoutePlanner.API.Models;

namespace TouristRoutePlanner.API.Data
{
    public class TouristRoutePlannerDbContext: DbContext
    {
        public TouristRoutePlannerDbContext(DbContextOptions<TouristRoutePlannerDbContext> dbContextOptions): base(dbContextOptions)
        {
            
        }

        public DbSet<Place> Places { get; set; }
        public DbSet<Models.Type> Types { get; set; }
        public DbSet<Travel> Travels { get; set; }
        public DbSet<PlaceType> PlaceTypes { get; set; }
        public DbSet<TravelType> TravelTypes { get; set; }
        public DbSet<TravelPlace> TravelPlaces { get; set; }
        public DbSet<Distance> Distances { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Place entity
            modelBuilder.Entity<Place>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.ExternalId).IsRequired();
                entity.Property(p => p.DisplayName).IsRequired();
                entity.Property(p => p.LanguageCode).IsRequired();
                entity.Property(p => p.Latitude).IsRequired();
                entity.Property(p => p.Longitude).IsRequired();

                // Add a unique index on ExternalId since it's used as a foreign key in Distance
                entity.HasIndex(p => p.ExternalId).IsUnique();
            });

            // Configure Type entity
            modelBuilder.Entity<Models.Type>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.Property(t => t.Name).IsRequired();
            });

            // Configure Travel entity
            modelBuilder.Entity<Travel>(entity =>
            {
                entity.HasKey(t => t.Id);
                // Add any other Travel-specific configurations here
            });

            // Configure PlaceType junction entity
            modelBuilder.Entity<PlaceType>(entity =>
            {
                entity.HasKey(pt => new { pt.PlaceId, pt.TypeId });

                entity.HasOne(pt => pt.Place)
                    .WithMany(p => p.PlaceTypes)
                    .HasForeignKey(pt => pt.PlaceId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(pt => pt.Type)
                    .WithMany(t => t.PlaceTypes)
                    .HasForeignKey(pt => pt.TypeId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure TravelType junction entity
            modelBuilder.Entity<TravelType>(entity =>
            {
                entity.HasKey(tt => new { tt.TravelId, tt.TypeId });

                entity.HasOne(tt => tt.Travel)
                    .WithMany(t => t.TravelTypes)
                    .HasForeignKey(tt => tt.TravelId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(tt => tt.Type)
                    .WithMany(t => t.TravelTypes)
                    .HasForeignKey(tt => tt.TypeId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure TravelPlace junction entity
            modelBuilder.Entity<TravelPlace>(entity =>
            {
                entity.HasKey(tp => new { tp.TravelId, tp.PlaceId });

                entity.HasOne(tp => tp.Travel)
                    .WithMany(t => t.TravelPlaces)
                    .HasForeignKey(tp => tp.TravelId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(tp => tp.Place)
                    .WithMany(p => p.TravelPlaces)
                    .HasForeignKey(tp => tp.PlaceId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure Distance entity
            modelBuilder.Entity<Distance>(entity =>
            {
                entity.HasKey(d => d.Id);

                entity.Property(d => d.OriginPlaceExternalId).IsRequired();
                entity.Property(d => d.DestinationPlaceExternalId).IsRequired();
                entity.Property(d => d.WalkingDistance).IsRequired();
                entity.Property(d => d.WalkingDuration).IsRequired();
                entity.Property(d => d.DrivingDistance).IsRequired();
                entity.Property(d => d.DrivingDuration).IsRequired();

                // Configure relationship with Place for origin and destination
                // using ExternalId as the foreign key
                entity.HasOne(d => d.OriginPlace)
                    .WithMany()
                    .HasForeignKey(d => d.OriginPlaceExternalId)
                    .HasPrincipalKey(p => p.ExternalId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.DestinationPlace)
                    .WithMany()
                    .HasForeignKey(d => d.DestinationPlaceExternalId)
                    .HasPrincipalKey(p => p.ExternalId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
