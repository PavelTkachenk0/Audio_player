using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Audio_player.DAL.Migrations
{
    /// <inheritdoc />
    public partial class T50 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AvatarPath",
                table: "Artists",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvatarPath",
                table: "Artists");
        }
    }
}
