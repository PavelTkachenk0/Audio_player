using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Audio_player.DAL.Migrations
{
    /// <inheritdoc />
    public partial class Fix_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "Own",
                table: "UserPlaylists",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AddColumn<bool>(
                name: "IsAdmin",
                table: "UserPlaylists",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Duration",
                table: "Songs",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "CoverPath",
                table: "Playlists",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAdmin",
                table: "UserPlaylists");

            migrationBuilder.DropColumn(
                name: "Duration",
                table: "Songs");

            migrationBuilder.DropColumn(
                name: "CoverPath",
                table: "Playlists");

            migrationBuilder.AlterColumn<bool>(
                name: "Own",
                table: "UserPlaylists",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);
        }
    }
}
