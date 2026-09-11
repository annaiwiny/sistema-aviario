using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FarmSystemProject.Data.Migrations
{
    /// <inheritdoc />
    public partial class SalesBelongToFarm : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sales_Lots_LotId",
                table: "Sales");

            migrationBuilder.AlterColumn<int>(
                name: "LotId",
                table: "Sales",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "FarmId",
                table: "Sales",
                type: "int",
                nullable: false,
                defaultValue: 0);

            // As vendas que ja existiam foram lancadas dentro de um lote. Elas
            // passam a pertencer a granja daquele lote; sem isso ficariam com
            // FarmId = 0 e a criacao da chave estrangeira abaixo falharia.
            migrationBuilder.Sql(@"
                UPDATE s
                   SET s.FarmId = l.FarmId
                  FROM Sales s
                 INNER JOIN Lots l ON l.Id = s.LotId;");

            migrationBuilder.CreateIndex(
                name: "IX_Sales_FarmId",
                table: "Sales",
                column: "FarmId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sales_Farms_FarmId",
                table: "Sales",
                column: "FarmId",
                principalTable: "Farms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Sales_Lots_LotId",
                table: "Sales",
                column: "LotId",
                principalTable: "Lots",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sales_Farms_FarmId",
                table: "Sales");

            migrationBuilder.DropForeignKey(
                name: "FK_Sales_Lots_LotId",
                table: "Sales");

            migrationBuilder.DropIndex(
                name: "IX_Sales_FarmId",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "FarmId",
                table: "Sales");

            // Vendas da granja nao tem lote. Voltando ao modelo antigo elas nao
            // teriam para onde ir, entao saem junto com a coluna.
            migrationBuilder.Sql("DELETE FROM Sales WHERE LotId IS NULL;");

            migrationBuilder.AlterColumn<int>(
                name: "LotId",
                table: "Sales",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Sales_Lots_LotId",
                table: "Sales",
                column: "LotId",
                principalTable: "Lots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
