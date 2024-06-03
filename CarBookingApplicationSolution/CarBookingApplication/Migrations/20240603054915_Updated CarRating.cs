using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarBookingApplication.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedCarRating : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BookingId",
                table: "CarRatings",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BookingId",
                table: "CarRatings");
        }
    }
}
