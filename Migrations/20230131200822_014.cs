using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Portfolio.Migrations
{
    /// <inheritdoc />
    public partial class _014 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Block_PostBodies_PostBodyId",
                table: "Block");

            migrationBuilder.AddColumn<string>(
                name: "PostBlockId",
                table: "PostBodies",
                type: "varchar(255)",
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<Guid>(
                name: "PostBodyId",
                table: "Block",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_PostBodies_PostBlockId",
                table: "PostBodies",
                column: "PostBlockId");

            migrationBuilder.AddForeignKey(
                name: "FK_Block_PostBodies_PostBodyId",
                table: "Block",
                column: "PostBodyId",
                principalTable: "PostBodies",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PostBodies_Block_PostBlockId",
                table: "PostBodies",
                column: "PostBlockId",
                principalTable: "Block",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Block_PostBodies_PostBodyId",
                table: "Block");

            migrationBuilder.DropForeignKey(
                name: "FK_PostBodies_Block_PostBlockId",
                table: "PostBodies");

            migrationBuilder.DropIndex(
                name: "IX_PostBodies_PostBlockId",
                table: "PostBodies");

            migrationBuilder.DropColumn(
                name: "PostBlockId",
                table: "PostBodies");

            migrationBuilder.AlterColumn<Guid>(
                name: "PostBodyId",
                table: "Block",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldNullable: true)
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AddForeignKey(
                name: "FK_Block_PostBodies_PostBodyId",
                table: "Block",
                column: "PostBodyId",
                principalTable: "PostBodies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
