using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AFRI_AusCare.Migrations
{
    /// <inheritdoc />
    public partial class GalleryEntityExtra : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Galleries",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Galleries",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDate",
                table: "Galleries",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Galleries");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Galleries");

            migrationBuilder.DropColumn(
                name: "ModifiedDate",
                table: "Galleries");
        }
    }
}
