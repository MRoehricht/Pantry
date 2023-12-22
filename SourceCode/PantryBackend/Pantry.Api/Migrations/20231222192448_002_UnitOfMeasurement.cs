using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pantry.Api.Migrations
{
    /// <inheritdoc />
    public partial class _002_UnitOfMeasurement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UnitOfMeasurement",
                table: "Goods",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UnitOfMeasurement",
                table: "Goods");
        }
    }
}
