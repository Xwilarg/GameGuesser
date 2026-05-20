using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameGuesser.Backend.Database.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Game",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LastUpdate = table.Column<string>(type: "TEXT", nullable: false),
                    Iteration = table.Column<int>(type: "INTEGER", nullable: false),
                    GameId = table.Column<int>(type: "INTEGER", nullable: false),
                    IsUpdating = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Game", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LocalGames",
                columns: table => new
                {
                    Language = table.Column<int>(type: "INTEGER", nullable: false),
                    SteamApi = table.Column<string>(type: "TEXT", nullable: true),
                    Game = table.Column<string>(type: "TEXT", nullable: true),
                    Verbs = table.Column<string>(type: "TEXT", nullable: true),
                    IsUpdating = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocalGames", x => x.Language);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Game");

            migrationBuilder.DropTable(
                name: "LocalGames");
        }
    }
}
