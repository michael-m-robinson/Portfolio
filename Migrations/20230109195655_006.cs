using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Portfolio.Migrations
{
    /// <inheritdoc />
    public partial class _006 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "ProjectLeadImage",
                table: "Projects",
                type: "longblob",
                nullable: false);

            migrationBuilder.AddColumn<string>(
                name: "ProjectLeadPictureContentType",
                table: "Projects",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProjectLeadImage",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "ProjectLeadPictureContentType",
                table: "Projects");
        }
    }
}
