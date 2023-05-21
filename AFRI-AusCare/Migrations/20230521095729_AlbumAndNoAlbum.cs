using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AFRI_AusCare.Migrations
{
    /// <inheritdoc />
    public partial class AlbumAndNoAlbum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAlbum",
                table: "Albums",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAlbum",
                table: "Albums");
        }
    }
}
