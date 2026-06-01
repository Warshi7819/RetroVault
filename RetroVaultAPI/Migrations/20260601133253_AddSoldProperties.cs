using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RetroVaultAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddSoldProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SalePrice",
                table: "VaultItems",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Sold",
                table: "VaultItems",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SalePrice",
                table: "VaultItems");

            migrationBuilder.DropColumn(
                name: "Sold",
                table: "VaultItems");
        }
    }
}
