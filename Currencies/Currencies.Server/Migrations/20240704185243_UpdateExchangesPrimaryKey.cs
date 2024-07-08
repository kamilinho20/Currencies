using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Currencies.Server.Migrations
{
    /// <inheritdoc />
    public partial class UpdateExchangesPrimaryKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Exchanges",
                table: "Exchanges");

            migrationBuilder.DropIndex(
                name: "IX_Exchanges_BaseCurrencyCode",
                table: "Exchanges");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Exchanges");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Exchanges",
                table: "Exchanges",
                columns: new[] { "BaseCurrencyCode", "ExchangeCurrencyCode", "ExDate" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Exchanges",
                table: "Exchanges");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Exchanges",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Exchanges",
                table: "Exchanges",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Exchanges_BaseCurrencyCode",
                table: "Exchanges",
                column: "BaseCurrencyCode");
        }
    }
}
