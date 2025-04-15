using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TouristRoutePlanner.API.Migrations.TouristRoutePlannerDb
{
    /// <inheritdoc />
    public partial class AddingCityFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Travels",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Places",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "City",
                table: "Travels");

            migrationBuilder.DropColumn(
                name: "City",
                table: "Places");
        }
    }
}
