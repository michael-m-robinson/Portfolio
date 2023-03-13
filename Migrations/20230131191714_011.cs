using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Portfolio.Migrations
{
    /// <inheritdoc />
    public partial class _011 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "blocksId",
                table: "PostBodies");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "blocksId",
                table: "PostBodies",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
