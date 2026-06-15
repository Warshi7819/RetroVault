using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RetroVaultAPI.Migrations
{
    /// <inheritdoc />
    public partial class PriceCharting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PriceChartingCompletePrice",
                table: "VaultItems",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "PriceChartingLastUpdated",
                table: "VaultItems",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "PriceChartingLoosePrice",
                table: "VaultItems",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "PriceChartingURL",
                table: "VaultItems",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PriceChartingCompletePrice",
                table: "VaultItems");

            migrationBuilder.DropColumn(
                name: "PriceChartingLastUpdated",
                table: "VaultItems");

            migrationBuilder.DropColumn(
                name: "PriceChartingLoosePrice",
                table: "VaultItems");

            migrationBuilder.DropColumn(
                name: "PriceChartingURL",
                table: "VaultItems");
        }
    }
}
