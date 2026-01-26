using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RetroVaultAPI.Migrations
{
    /// <inheritdoc />
    public partial class changedschemastillindev : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "VideoFolder",
                table: "VaultItems",
                newName: "StorageLocation");

            migrationBuilder.RenameColumn(
                name: "Price",
                table: "VaultItems",
                newName: "Region");

            migrationBuilder.RenameColumn(
                name: "PhysicalLocation",
                table: "VaultItems",
                newName: "Currency");

            migrationBuilder.RenameColumn(
                name: "ImageFolder",
                table: "VaultItems",
                newName: "Completeness");

            migrationBuilder.RenameColumn(
                name: "DocumentationFolder",
                table: "VaultItems",
                newName: "AcquiredFrom");

            migrationBuilder.RenameColumn(
                name: "Currencty",
                table: "VaultItems",
                newName: "AcquiredDate");

            migrationBuilder.AddColumn<int>(
                name: "PurchasePrice",
                table: "VaultItems",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PurchasePrice",
                table: "VaultItems");

            migrationBuilder.RenameColumn(
                name: "StorageLocation",
                table: "VaultItems",
                newName: "VideoFolder");

            migrationBuilder.RenameColumn(
                name: "Region",
                table: "VaultItems",
                newName: "Price");

            migrationBuilder.RenameColumn(
                name: "Currency",
                table: "VaultItems",
                newName: "PhysicalLocation");

            migrationBuilder.RenameColumn(
                name: "Completeness",
                table: "VaultItems",
                newName: "ImageFolder");

            migrationBuilder.RenameColumn(
                name: "AcquiredFrom",
                table: "VaultItems",
                newName: "DocumentationFolder");

            migrationBuilder.RenameColumn(
                name: "AcquiredDate",
                table: "VaultItems",
                newName: "Currencty");
        }
    }
}
