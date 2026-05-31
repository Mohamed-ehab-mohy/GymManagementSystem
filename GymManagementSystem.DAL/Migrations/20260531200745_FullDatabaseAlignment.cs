using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GymManagementSystem.DAL.Migrations
{
    /// <inheritdoc />
    public partial class FullDatabaseAlignment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_GymUser_Phone",
                table: "GymUsers");

            migrationBuilder.DropCheckConstraint(
                name: "CK_ClassSession_Capacity",
                table: "ClassSessions");

            migrationBuilder.DropColumn(
                name: "Specialization",
                table: "GymUsers");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "Plans",
                type: "decimal(10,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<string>(
                name: "MedicalConditions",
                table: "HealthRecords",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdate",
                table: "HealthRecords",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()");

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "HealthRecords",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "GymUsers",
                type: "nvarchar(11)",
                maxLength: 11,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfBirth",
                table: "GymUsers",
                type: "date",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Gender",
                table: "GymUsers",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "JoinDate",
                table: "GymUsers",
                type: "datetime2",
                nullable: true,
                defaultValueSql: "GETDATE()");

            migrationBuilder.AddColumn<string>(
                name: "Photo",
                table: "GymUsers",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Specialty",
                table: "GymUsers",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "ClassSessions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "CategoryName", "CreatedAt", "CreatedBy", "DeletedAt", "IsDeleted", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, "Cardio", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, false, null, null },
                    { 2, "Strength", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, false, null, null },
                    { 3, "Yoga", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, false, null, null },
                    { 4, "Boxing", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, false, null, null },
                    { 5, "CrossFit", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, false, null, null }
                });

            migrationBuilder.AddCheckConstraint(
                name: "CK_Plan_DurationDays",
                table: "Plans",
                sql: "DurationDays >= 1 AND DurationDays <= 365");

            migrationBuilder.CreateIndex(
                name: "IX_GymUsers_Email",
                table: "GymUsers",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GymUsers_PhoneNumber",
                table: "GymUsers",
                column: "PhoneNumber",
                unique: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_GymUser_Phone",
                table: "GymUsers",
                sql: "PhoneNumber LIKE '010[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]' OR PhoneNumber LIKE '011[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]' OR PhoneNumber LIKE '012[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]' OR PhoneNumber LIKE '015[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]'");

            migrationBuilder.CreateIndex(
                name: "IX_ClassSessions_CategoryId",
                table: "ClassSessions",
                column: "CategoryId");

            migrationBuilder.AddCheckConstraint(
                name: "CK_ClassSession_Capacity",
                table: "ClassSessions",
                sql: "Capacity > 0 AND Capacity <= 25");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassSessions_Categories_CategoryId",
                table: "ClassSessions",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassSessions_Categories_CategoryId",
                table: "ClassSessions");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Plan_DurationDays",
                table: "Plans");

            migrationBuilder.DropIndex(
                name: "IX_GymUsers_Email",
                table: "GymUsers");

            migrationBuilder.DropIndex(
                name: "IX_GymUsers_PhoneNumber",
                table: "GymUsers");

            migrationBuilder.DropCheckConstraint(
                name: "CK_GymUser_Phone",
                table: "GymUsers");

            migrationBuilder.DropIndex(
                name: "IX_ClassSessions_CategoryId",
                table: "ClassSessions");

            migrationBuilder.DropCheckConstraint(
                name: "CK_ClassSession_Capacity",
                table: "ClassSessions");

            migrationBuilder.DropColumn(
                name: "LastUpdate",
                table: "HealthRecords");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "HealthRecords");

            migrationBuilder.DropColumn(
                name: "DateOfBirth",
                table: "GymUsers");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "GymUsers");

            migrationBuilder.DropColumn(
                name: "JoinDate",
                table: "GymUsers");

            migrationBuilder.DropColumn(
                name: "Photo",
                table: "GymUsers");

            migrationBuilder.DropColumn(
                name: "Specialty",
                table: "GymUsers");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "ClassSessions");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "Plans",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)");

            migrationBuilder.AlterColumn<string>(
                name: "MedicalConditions",
                table: "HealthRecords",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "GymUsers",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(11)",
                oldMaxLength: 11);

            migrationBuilder.AddColumn<string>(
                name: "Specialization",
                table: "GymUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_GymUser_Phone",
                table: "GymUsers",
                sql: "PhoneNumber NOT LIKE '%[^0-9]%'");

            migrationBuilder.AddCheckConstraint(
                name: "CK_ClassSession_Capacity",
                table: "ClassSessions",
                sql: "Capacity > 0 AND Capacity <= 100");
        }
    }
}
