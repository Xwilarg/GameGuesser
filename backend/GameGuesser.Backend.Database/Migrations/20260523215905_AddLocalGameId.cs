using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameGuesser.Backend.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddLocalGameId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GameId",
                table: "LocalGames",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GameId",
                table: "LocalGames");
        }
    }
}
