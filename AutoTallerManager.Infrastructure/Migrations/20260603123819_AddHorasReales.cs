using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoTallerManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddHorasReales : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrdenesServicio_ServiciosTaller_ServicioTallerId",
                table: "OrdenesServicio");

            migrationBuilder.DropIndex(
                name: "IX_OrdenesServicio_ServicioTallerId",
                table: "OrdenesServicio");

            migrationBuilder.RenameColumn(
                name: "ServicioTallerId",
                table: "OrdenesServicio",
                newName: "MecanicoId");

            migrationBuilder.AddColumn<int>(
                name: "HorasReales",
                table: "DetallesOrdenServicio",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HorasReales",
                table: "DetallesOrdenServicio");

            migrationBuilder.RenameColumn(
                name: "MecanicoId",
                table: "OrdenesServicio",
                newName: "ServicioTallerId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesServicio_ServicioTallerId",
                table: "OrdenesServicio",
                column: "ServicioTallerId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrdenesServicio_ServiciosTaller_ServicioTallerId",
                table: "OrdenesServicio",
                column: "ServicioTallerId",
                principalTable: "ServiciosTaller",
                principalColumn: "Id");
        }
    }
}
