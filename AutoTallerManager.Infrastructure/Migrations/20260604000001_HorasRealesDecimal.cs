using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoTallerManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class HorasRealesDecimal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "HorasReales",
                table: "DetallesOrdenServicio",
                type: "decimal(10,2)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "HorasReales",
                table: "DetallesOrdenServicio",
                type: "integer",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldNullable: true);
        }
    }
}
