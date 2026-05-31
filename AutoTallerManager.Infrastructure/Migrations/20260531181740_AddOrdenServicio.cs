using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoTallerManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOrdenServicio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrdenesServicio_CitasTaller_IdCita",
                table: "OrdenesServicio");

            migrationBuilder.DropForeignKey(
                name: "FK_OrdenesServicio_Usuarios_IdMecanico",
                table: "OrdenesServicio");

            migrationBuilder.DropForeignKey(
                name: "FK_OrdenesServicio_Vehiculos_IdVehiculo",
                table: "OrdenesServicio");

            migrationBuilder.DropIndex(
                name: "IX_OrdenesServicio_IdCita",
                table: "OrdenesServicio");

            migrationBuilder.DropIndex(
                name: "IX_OrdenesServicio_IdMecanico",
                table: "OrdenesServicio");

            migrationBuilder.DropColumn(
                name: "FechaEstimadaEntrega",
                table: "OrdenesServicio");

            migrationBuilder.DropColumn(
                name: "IdCita",
                table: "OrdenesServicio");

            migrationBuilder.DropColumn(
                name: "IdMecanico",
                table: "OrdenesServicio");

            migrationBuilder.DropColumn(
                name: "TipoServicio",
                table: "OrdenesServicio");

            migrationBuilder.RenameColumn(
                name: "IdVehiculo",
                table: "OrdenesServicio",
                newName: "VehiculoId");

            migrationBuilder.RenameIndex(
                name: "IX_OrdenesServicio_IdVehiculo",
                table: "OrdenesServicio",
                newName: "IX_OrdenesServicio_VehiculoId");

            migrationBuilder.AlterColumn<string>(
                name: "Estado",
                table: "OrdenesServicio",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldMaxLength: 30,
                oldDefaultValue: "Pendiente");

            migrationBuilder.AddColumn<decimal>(
                name: "CostoEstimado",
                table: "OrdenesServicio",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CostoTotal",
                table: "OrdenesServicio",
                type: "numeric(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DescripcionProblema",
                table: "OrdenesServicio",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DiagnosticoMecanico",
                table: "OrdenesServicio",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaEntrega",
                table: "OrdenesServicio",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_OrdenesServicio_Vehiculos_VehiculoId",
                table: "OrdenesServicio",
                column: "VehiculoId",
                principalTable: "Vehiculos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrdenesServicio_Vehiculos_VehiculoId",
                table: "OrdenesServicio");

            migrationBuilder.DropColumn(
                name: "CostoEstimado",
                table: "OrdenesServicio");

            migrationBuilder.DropColumn(
                name: "CostoTotal",
                table: "OrdenesServicio");

            migrationBuilder.DropColumn(
                name: "DescripcionProblema",
                table: "OrdenesServicio");

            migrationBuilder.DropColumn(
                name: "DiagnosticoMecanico",
                table: "OrdenesServicio");

            migrationBuilder.DropColumn(
                name: "FechaEntrega",
                table: "OrdenesServicio");

            migrationBuilder.RenameColumn(
                name: "VehiculoId",
                table: "OrdenesServicio",
                newName: "IdVehiculo");

            migrationBuilder.RenameIndex(
                name: "IX_OrdenesServicio_VehiculoId",
                table: "OrdenesServicio",
                newName: "IX_OrdenesServicio_IdVehiculo");

            migrationBuilder.AlterColumn<string>(
                name: "Estado",
                table: "OrdenesServicio",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "Pendiente",
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldMaxLength: 30);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaEstimadaEntrega",
                table: "OrdenesServicio",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "IdCita",
                table: "OrdenesServicio",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IdMecanico",
                table: "OrdenesServicio",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "TipoServicio",
                table: "OrdenesServicio",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesServicio_IdCita",
                table: "OrdenesServicio",
                column: "IdCita",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesServicio_IdMecanico",
                table: "OrdenesServicio",
                column: "IdMecanico");

            migrationBuilder.AddForeignKey(
                name: "FK_OrdenesServicio_CitasTaller_IdCita",
                table: "OrdenesServicio",
                column: "IdCita",
                principalTable: "CitasTaller",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_OrdenesServicio_Usuarios_IdMecanico",
                table: "OrdenesServicio",
                column: "IdMecanico",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrdenesServicio_Vehiculos_IdVehiculo",
                table: "OrdenesServicio",
                column: "IdVehiculo",
                principalTable: "Vehiculos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
