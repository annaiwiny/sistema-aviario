using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FarmSystemProject.Data.Migrations
{
    /// <inheritdoc />
    public partial class EggProductionTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CollectEggs");

            migrationBuilder.CreateTable(
                name: "EggProductions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    LotId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EggProductions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EggProductions_Lots_LotId",
                        column: x => x.LotId,
                        principalTable: "Lots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EggProductions_LotId",
                table: "EggProductions",
                column: "LotId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EggProductions");

            migrationBuilder.CreateTable(
                name: "CollectEggs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LotId = table.Column<int>(type: "int", nullable: false),
                    CollectDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CollectQuantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CollectEggs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CollectEggs_Lots_LotId",
                        column: x => x.LotId,
                        principalTable: "Lots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CollectEggs_LotId",
                table: "CollectEggs",
                column: "LotId");
        }
    }
}
