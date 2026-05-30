using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymManagementSystem.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddCheckedInAtToBooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CheckedInAt",
                table: "Bookings",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAttended",
                table: "Bookings",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CheckedInAt",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "IsAttended",
                table: "Bookings");
        }
    }
}
