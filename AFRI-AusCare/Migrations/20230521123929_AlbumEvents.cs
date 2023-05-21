using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AFRI_AusCare.Migrations
{
    /// <inheritdoc />
    public partial class AlbumEvents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AlbumId",
                table: "Events",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Events_AlbumId",
                table: "Events",
                column: "AlbumId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Albums_AlbumId",
                table: "Events",
                column: "AlbumId",
                principalTable: "Albums",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_Albums_AlbumId",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_Events_AlbumId",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "AlbumId",
                table: "Events");
        }
    }
}
