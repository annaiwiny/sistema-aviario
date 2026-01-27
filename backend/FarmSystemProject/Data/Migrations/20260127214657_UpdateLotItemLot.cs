using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FarmSystemProject.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateLotItemLot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Race",
                table: "Lots");

            migrationBuilder.DropColumn(
                name: "RaceQuantity",
                table: "Lots");

            migrationBuilder.CreateTable(
                name: "LotItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Race = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    LotId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LotItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LotItems_Lots_LotId",
                        column: x => x.LotId,
                        principalTable: "Lots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LotItems_LotId",
                table: "LotItems",
                column: "LotId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LotItems");

            migrationBuilder.AddColumn<string>(
                name: "Race",
                table: "Lots",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "RaceQuantity",
                table: "Lots",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
