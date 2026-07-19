using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PharmacyChainBe.Migrations
{
    /// <inheritdoc />
    public partial class AddImportPriceToMedicine : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ImportPrice",
                table: "Medicines",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImportPrice",
                table: "Medicines");
        }
    }
}
