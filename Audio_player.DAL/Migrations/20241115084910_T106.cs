using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Audio_player.DAL.Migrations
{
    /// <inheritdoc />
    public partial class T106 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAdmin",
                table: "UserPlaylists");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAdmin",
                table: "UserPlaylists",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
