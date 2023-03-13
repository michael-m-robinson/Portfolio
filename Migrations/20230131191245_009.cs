using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Portfolio.Migrations
{
    /// <inheritdoc />
    public partial class _009 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Slug",
                table: "Projects",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<Guid>(
                name: "PostBodyId",
                table: "Posts",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.CreateTable(
                name: "File",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    url = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_File", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PostBodies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    blocksId = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    time = table.Column<long>(type: "bigint", nullable: false),
                    version = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostBodies", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Data",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    text = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    level = table.Column<int>(type: "int", nullable: true),
                    caption = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    alignment = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    fileId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    withBorder = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    withBackground = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    stretched = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    code = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Data", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Data_File_fileId",
                        column: x => x.fileId,
                        principalTable: "File",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Block",
                columns: table => new
                {
                    id = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    type = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    dataId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    PostBodyId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Block", x => x.id);
                    table.ForeignKey(
                        name: "FK_Block_Data_dataId",
                        column: x => x.dataId,
                        principalTable: "Data",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Block_PostBodies_PostBodyId",
                        column: x => x.PostBodyId,
                        principalTable: "PostBodies",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_PostBodyId",
                table: "Posts",
                column: "PostBodyId");

            migrationBuilder.CreateIndex(
                name: "IX_Block_dataId",
                table: "Block",
                column: "dataId");

            migrationBuilder.CreateIndex(
                name: "IX_Block_PostBodyId",
                table: "Block",
                column: "PostBodyId");

            migrationBuilder.CreateIndex(
                name: "IX_Data_fileId",
                table: "Data",
                column: "fileId");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_PostBodies_PostBodyId",
                table: "Posts",
                column: "PostBodyId",
                principalTable: "PostBodies",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_PostBodies_PostBodyId",
                table: "Posts");

            migrationBuilder.DropTable(
                name: "Block");

            migrationBuilder.DropTable(
                name: "Data");

            migrationBuilder.DropTable(
                name: "PostBodies");

            migrationBuilder.DropTable(
                name: "File");

            migrationBuilder.DropIndex(
                name: "IX_Posts_PostBodyId",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "PostBodyId",
                table: "Posts");

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Slug",
                keyValue: null,
                column: "Slug",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "Slug",
                table: "Projects",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
