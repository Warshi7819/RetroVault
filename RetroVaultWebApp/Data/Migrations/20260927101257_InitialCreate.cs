using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RetroVaultWebApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Themes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    IsBuiltIn = table.Column<bool>(type: "INTEGER", nullable: false),
                    BodyBg = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    BodyColor = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    CardBg = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    CardBorderColor = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    PrimaryColor = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    NavbarBg = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    NavbarTextColor = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    FooterBg = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    MutedColor = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Themes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Username = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    IsAdmin = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsDisabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    Alias = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    PreferredCurrency = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false, defaultValue: "NOK"),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Themes",
                columns: new[] { "Id", "BodyBg", "BodyColor", "CardBg", "CardBorderColor", "FooterBg", "IsBuiltIn", "MutedColor", "Name", "NavbarBg", "NavbarTextColor", "PrimaryColor", "UserId" },
                values: new object[,]
                {
                    { 1, "#ffffff", "#212529", "#ffffff", "rgba(0,0,0,0.125)", "#f8f9fa", true, "#6c757d", "Light", "#0d6efd", "#ffffff", "#0d6efd", null },
                    { 2, "#ffffff", "#212529", "#ffffff", "rgba(0,0,0,0.125)", "#f8f9fa", true, "#6c757d", "Dark", "#0d6efd", "#ffffff", "#0d6efd", null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Themes");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
