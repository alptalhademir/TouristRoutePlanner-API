using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TouristRoutePlanner.API.Migrations.TouristRoutePlannerDb
{
    /// <inheritdoc />
    public partial class RemoveTravelerTypeId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TravelerTypeId",
                table: "Travels");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TravelerTypeId",
                table: "Travels",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}
