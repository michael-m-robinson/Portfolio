using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Portfolio.Migrations
{
    /// <inheritdoc />
    public partial class _016 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Block_Data_dataId",
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

            migrationBuilder.RenameColumn(
                name: "version",
                table: "PostBodies",
                newName: "Version");

            migrationBuilder.RenameColumn(
                name: "time",
                table: "PostBodies",
                newName: "Time");

            migrationBuilder.RenameColumn(
                name: "url",
                table: "File",
                newName: "Url");

            migrationBuilder.RenameColumn(
                name: "type",
                table: "Block",
                newName: "Type");

            migrationBuilder.RenameColumn(
                name: "dataId",
                table: "Block",
                newName: "DataId");

            migrationBuilder.RenameIndex(
                name: "IX_Block_dataId",
                table: "Block",
                newName: "IX_Block_DataId");

            migrationBuilder.AddForeignKey(
                name: "FK_Block_Data_DataId",
                table: "Block",
                column: "DataId",
                principalTable: "Data",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Block_Data_DataId",
                table: "Block");

            migrationBuilder.RenameColumn(
                name: "Version",
                table: "PostBodies",
                newName: "version");

            migrationBuilder.RenameColumn(
                name: "Time",
                table: "PostBodies",
                newName: "time");

            migrationBuilder.RenameColumn(
                name: "Url",
                table: "File",
                newName: "url");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "Block",
                newName: "type");

            migrationBuilder.RenameColumn(
                name: "DataId",
                table: "Block",
                newName: "dataId");

            migrationBuilder.RenameIndex(
                name: "IX_Block_DataId",
                table: "Block",
                newName: "IX_Block_dataId");

            migrationBuilder.AddColumn<string>(
                name: "PostBlockId",
                table: "PostBodies",
                type: "varchar(255)",
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_PostBodies_PostBlockId",
                table: "PostBodies",
                column: "PostBlockId");

            migrationBuilder.AddForeignKey(
                name: "FK_Block_Data_dataId",
                table: "Block",
                column: "dataId",
                principalTable: "Data",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PostBodies_Block_PostBlockId",
                table: "PostBodies",
                column: "PostBlockId",
                principalTable: "Block",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
