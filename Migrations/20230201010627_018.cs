using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Portfolio.Migrations
{
    /// <inheritdoc />
    public partial class _018 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Content",
                table: "Posts");

            migrationBuilder.AlterColumn<byte[]>(
                name: "PostBytes",
                table: "Posts",
                type: "longblob",
                nullable: true,
                oldClrType: typeof(byte[]),
                oldType: "longblob");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte[]>(
                name: "PostBytes",
                table: "Posts",
                type: "longblob",
                nullable: false,
                defaultValue: new byte[0],
                oldClrType: typeof(byte[]),
                oldType: "longblob",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Content",
                table: "Posts",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
