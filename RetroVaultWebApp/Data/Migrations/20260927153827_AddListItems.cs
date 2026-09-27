using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RetroVaultWebApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddListItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ListItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ListType = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ListItems", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "ListItems",
                columns: new[] { "Id", "ListType", "Name", "SortOrder" },
                values: new object[,]
                {
                    { 1, "Category", "Hardware", 0 },
                    { 2, "Category", "Peripherals", 1 },
                    { 3, "Category", "Games", 2 },
                    { 4, "Category", "Software", 3 },
                    { 5, "Category", "LP", 4 },
                    { 6, "Category", "Books", 5 },
                    { 7, "Category", "Magazines", 6 },
                    { 8, "Category", "Movies", 7 },
                    { 9, "Category", "Other", 8 },
                    { 10, "System", "PC", 0 },
                    { 11, "System", "PC Engine/TurboGrafx", 1 },
                    { 12, "System", "Commodore 64/128", 2 },
                    { 13, "System", "Commodore Amiga", 3 },
                    { 14, "System", "Commodore 16/Plus4", 4 },
                    { 15, "System", "Atari 2600", 5 },
                    { 16, "System", "Atari 800XL", 6 },
                    { 17, "System", "Atari 7800", 7 },
                    { 18, "System", "Atari ST", 8 },
                    { 19, "System", "Xbox", 9 },
                    { 20, "System", "Xbox 360", 10 },
                    { 21, "System", "Xbox One", 11 },
                    { 22, "System", "Game & Watch", 12 },
                    { 23, "System", "MSX", 13 },
                    { 24, "System", "Playstation 1", 14 },
                    { 25, "System", "Playstation 2", 15 },
                    { 26, "System", "Playstation 3", 16 },
                    { 27, "System", "Playstation 4", 17 },
                    { 28, "System", "Playstation Portable (PSP)", 18 },
                    { 29, "System", "Sega Master System", 19 },
                    { 30, "System", "Sega Mega Drive", 20 },
                    { 31, "System", "Sega Genesis", 21 },
                    { 32, "System", "NES", 22 },
                    { 33, "System", "Famicom", 23 },
                    { 34, "System", "SNES", 24 },
                    { 35, "System", "Super Famicom", 25 },
                    { 36, "System", "Nintendo 64", 26 },
                    { 37, "System", "GameBoy", 27 },
                    { 38, "System", "GameBoy Color", 28 },
                    { 39, "System", "GameBoy Advance", 29 },
                    { 40, "System", "Nintendo DS", 30 },
                    { 41, "System", "Nintendo Switch", 31 },
                    { 42, "System", "GameCube", 32 },
                    { 43, "System", "Tandy/Radio Shack", 33 },
                    { 44, "System", "Evercade", 34 },
                    { 45, "System", "WII", 35 },
                    { 46, "System", "Other", 36 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ListItems_ListType_Name",
                table: "ListItems",
                columns: new[] { "ListType", "Name" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ListItems");
        }
    }
}
