using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FarmSystemProject.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddFeedTotalCostAndRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LotId",
                table: "Feeds",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Feeds_LotId",
                table: "Feeds",
                column: "LotId");

            migrationBuilder.AddForeignKey(
                name: "FK_Feeds_Lots_LotId",
                table: "Feeds",
                column: "LotId",
                principalTable: "Lots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Feeds_Lots_LotId",
                table: "Feeds");

            migrationBuilder.DropIndex(
                name: "IX_Feeds_LotId",
                table: "Feeds");

            migrationBuilder.DropColumn(
                name: "LotId",
                table: "Feeds");
        }
    }
}
