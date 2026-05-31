using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AutoTallerManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Aseguradoras",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RfcONit = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    RazonSocial = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    ContactoEmergencia = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false, defaultValue: ""),
                    CorreoCorporativo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Activa = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Aseguradoras", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BahiasServicio",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NombreBahia = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    UbicacionFisica = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    EstadoDisponible = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BahiasServicio", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Telefono = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Correo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HerramientaCategorias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false, defaultValue: "")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HerramientaCategorias", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InsumosTaller",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NombreInsumo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    StockActual = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    StockMinimoAlerta = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    UnidadMedida = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InsumosTaller", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MediosPago",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PermiteCuotas = table.Column<bool>(type: "boolean", nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MediosPago", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Proveedor",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RucONit = table.Column<string>(type: "text", nullable: false),
                    RazonSocial = table.Column<string>(type: "text", nullable: false),
                    ContactoNombre = table.Column<string>(type: "text", nullable: false),
                    Telefono = table.Column<string>(type: "text", nullable: false),
                    Correo = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Proveedor", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Repuestos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Codigo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Stock = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    PrecioUnitario = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Repuestos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServiciosTaller",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    TarifaBaseManoObra = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiciosTaller", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TokenBlocklist",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TokenHash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    FechaRevocacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    FechaExpiracionOriginal = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TokenBlocklist", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Correo = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    ContrasenaHash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Rol = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AseguradoraCoberturas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AseguradoraId = table.Column<int>(type: "integer", nullable: false),
                    CodigoCobertura = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    PorcentajeDeducible = table.Column<decimal>(type: "numeric(5,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AseguradoraCoberturas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AseguradoraCoberturas_Aseguradoras_AseguradoraId",
                        column: x => x.AseguradoraId,
                        principalTable: "Aseguradoras",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BahiaHistorialEstados",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdBahiaServicio = table.Column<int>(type: "integer", nullable: false),
                    Estado = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    FechaCambio = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    Observaciones = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false, defaultValue: "")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BahiaHistorialEstados", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BahiaHistorialEstados_BahiasServicio_IdBahiaServicio",
                        column: x => x.IdBahiaServicio,
                        principalTable: "BahiasServicio",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Vehiculos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdCliente = table.Column<int>(type: "integer", nullable: false),
                    Marca = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Modelo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Anio = table.Column<int>(type: "integer", nullable: false),
                    VIN = table.Column<string>(type: "character varying(17)", maxLength: 17, nullable: false),
                    EquipamientoJson = table.Column<string>(type: "jsonb", nullable: false, defaultValue: "{}")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vehiculos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vehiculos_Clientes_IdCliente",
                        column: x => x.IdCliente,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Herramientas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CodigoActivo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Marca = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValue: ""),
                    EstadoOperativo = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false, defaultValue: "Disponible"),
                    RequiereCalibracion = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CategoriaId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Herramientas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Herramientas_HerramientaCategorias_CategoriaId",
                        column: x => x.CategoriaId,
                        principalTable: "HerramientaCategorias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InsumoConsumoHistorial",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    InsumoTallerId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InsumoConsumoHistorial", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InsumoConsumoHistorial_InsumosTaller_InsumoTallerId",
                        column: x => x.InsumoTallerId,
                        principalTable: "InsumosTaller",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrdenesCompra",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProveedorId = table.Column<int>(type: "integer", nullable: false),
                    CodigoOrden = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    FechaEmision = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    EstadoOrden = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false, defaultValue: "Solicitado")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdenesCompra", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrdenesCompra_Proveedor_ProveedorId",
                        column: x => x.ProveedorId,
                        principalTable: "Proveedor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProveedorContactos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdProveedor = table.Column<int>(type: "integer", nullable: false),
                    Nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Cargo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValue: ""),
                    Telefono = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Correo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false, defaultValue: "")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProveedorContactos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProveedorContactos_Proveedor_IdProveedor",
                        column: x => x.IdProveedor,
                        principalTable: "Proveedor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProveedorRepuestos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProveedorId = table.Column<int>(type: "integer", nullable: false),
                    RepuestoId = table.Column<int>(type: "integer", nullable: false),
                    CostoCompraCotizado = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    TiempoEntregaEstimado = table.Column<TimeSpan>(type: "interval", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProveedorRepuestos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProveedorRepuestos_Proveedor_ProveedorId",
                        column: x => x.ProveedorId,
                        principalTable: "Proveedor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProveedorRepuestos_Repuestos_RepuestoId",
                        column: x => x.RepuestoId,
                        principalTable: "Repuestos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RepuestoUbicaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RepuestoId = table.Column<int>(type: "integer", nullable: false),
                    Bodega = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Estante = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CapacidadMaxima = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RepuestoUbicaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RepuestoUbicaciones_Repuestos_RepuestoId",
                        column: x => x.RepuestoId,
                        principalTable: "Repuestos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AuditoriaTransacciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UsuarioId = table.Column<int>(type: "integer", nullable: true),
                    EntidadAfectada = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    TipoAccion = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    DetalleDatos = table.Column<string>(type: "jsonb", nullable: false, defaultValue: "{}"),
                    FechaHora = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditoriaTransacciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuditoriaTransacciones_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "BahiaMecanicoAsignaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BahiaServicioId = table.Column<int>(type: "integer", nullable: false),
                    UsuarioMecanicoId = table.Column<int>(type: "integer", nullable: false),
                    FechaAsignacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BahiaMecanicoAsignaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BahiaMecanicoAsignaciones_BahiasServicio_BahiaServicioId",
                        column: x => x.BahiaServicioId,
                        principalTable: "BahiasServicio",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BahiaMecanicoAsignaciones_Usuarios_UsuarioMecanicoId",
                        column: x => x.UsuarioMecanicoId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CierresContablesDiarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FechaCierre = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UsuarioAdministradorId = table.Column<int>(type: "integer", nullable: false),
                    TotalEsperadoSistema = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    TotalRealFisico = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Diferencia = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Observaciones = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false, defaultValue: "")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CierresContablesDiarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CierresContablesDiarios_Usuarios_UsuarioAdministradorId",
                        column: x => x.UsuarioAdministradorId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MecanicoCertificaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UsuarioId = table.Column<int>(type: "integer", nullable: false),
                    NombreCertificacion = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    EnteEmisor = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    FechaExpiracion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MecanicoCertificaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MecanicoCertificaciones_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MecanicoEspecialidades",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UsuarioId = table.Column<int>(type: "integer", nullable: false),
                    Especialidad = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    NivelExperiencia = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MecanicoEspecialidades", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MecanicoEspecialidades_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UsuarioControlAccesos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UsuarioId = table.Column<int>(type: "integer", nullable: false),
                    FechaEvento = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    TipoEvento = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    DireccionIP = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: false),
                    DispositivoNavegador = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false, defaultValue: "")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuarioControlAccesos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UsuarioControlAccesos_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UsuarioHorarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UsuarioId = table.Column<int>(type: "integer", nullable: false),
                    DiaSemana = table.Column<int>(type: "integer", nullable: false),
                    HoraInicio = table.Column<TimeSpan>(type: "interval", nullable: false),
                    HoraFin = table.Column<TimeSpan>(type: "interval", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuarioHorarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UsuarioHorarios_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CitasTaller",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ClienteId = table.Column<int>(type: "integer", nullable: false),
                    VehiculoId = table.Column<int>(type: "integer", nullable: false),
                    ServicioTallerId = table.Column<int>(type: "integer", nullable: false),
                    FechaHoraReserva = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EstadoCita = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false, defaultValue: "Programada"),
                    NotasSintomas = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false, defaultValue: "")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CitasTaller", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CitasTaller_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CitasTaller_ServiciosTaller_ServicioTallerId",
                        column: x => x.ServicioTallerId,
                        principalTable: "ServiciosTaller",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CitasTaller_Vehiculos_VehiculoId",
                        column: x => x.VehiculoId,
                        principalTable: "Vehiculos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HistorialesKilometraje",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    VehiculoId = table.Column<int>(type: "integer", nullable: false),
                    Kilometraje = table.Column<int>(type: "integer", nullable: false),
                    FechaLectura = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    OrigenLectura = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistorialesKilometraje", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HistorialesKilometraje_Vehiculos_VehiculoId",
                        column: x => x.VehiculoId,
                        principalTable: "Vehiculos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HerramientaMantenimientos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    HerramientaId = table.Column<int>(type: "integer", nullable: false),
                    FechaMantenimiento = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TipoMantenimiento = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    DescripcionTrabajo = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false, defaultValue: ""),
                    CostoMantenimiento = table.Column<decimal>(type: "numeric(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HerramientaMantenimientos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HerramientaMantenimientos_Herramientas_HerramientaId",
                        column: x => x.HerramientaId,
                        principalTable: "Herramientas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DetallesOrdenCompra",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdOrdenCompra = table.Column<int>(type: "integer", nullable: false),
                    IdRepuesto = table.Column<int>(type: "integer", nullable: false),
                    CostoCompraPactado = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    CantidadSolicitada = table.Column<int>(type: "integer", nullable: false),
                    CantidadRecibida = table.Column<int>(type: "integer", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetallesOrdenCompra", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DetallesOrdenCompra_OrdenesCompra_IdOrdenCompra",
                        column: x => x.IdOrdenCompra,
                        principalTable: "OrdenesCompra",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DetallesOrdenCompra_Repuestos_IdRepuesto",
                        column: x => x.IdRepuesto,
                        principalTable: "Repuestos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrdenesServicio",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdVehiculo = table.Column<int>(type: "integer", nullable: false),
                    IdMecanico = table.Column<int>(type: "integer", nullable: false),
                    IdCita = table.Column<int>(type: "integer", nullable: true),
                    TipoServicio = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Estado = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false, defaultValue: "Pendiente"),
                    FechaIngreso = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    FechaEstimadaEntrega = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ServicioTallerId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdenesServicio", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrdenesServicio_CitasTaller_IdCita",
                        column: x => x.IdCita,
                        principalTable: "CitasTaller",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_OrdenesServicio_ServiciosTaller_ServicioTallerId",
                        column: x => x.ServicioTallerId,
                        principalTable: "ServiciosTaller",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_OrdenesServicio_Usuarios_IdMecanico",
                        column: x => x.IdMecanico,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdenesServicio_Vehiculos_IdVehiculo",
                        column: x => x.IdVehiculo,
                        principalTable: "Vehiculos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DetallesOrdenRepuesto",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrdenServicioId = table.Column<int>(type: "integer", nullable: false),
                    RepuestoId = table.Column<int>(type: "integer", nullable: false),
                    Cantidad = table.Column<int>(type: "integer", nullable: false),
                    PrecioVentaHistorico = table.Column<decimal>(type: "numeric(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetallesOrdenRepuesto", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DetallesOrdenRepuesto_OrdenesServicio_OrdenServicioId",
                        column: x => x.OrdenServicioId,
                        principalTable: "OrdenesServicio",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DetallesOrdenRepuesto_Repuestos_RepuestoId",
                        column: x => x.RepuestoId,
                        principalTable: "Repuestos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DetallesOrdenServicio",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdOrdenServicio = table.Column<int>(type: "integer", nullable: false),
                    IdServicioTaller = table.Column<int>(type: "integer", nullable: false),
                    PrecioManoObraHistorico = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    HorasEstimadas = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetallesOrdenServicio", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DetallesOrdenServicio_OrdenesServicio_IdOrdenServicio",
                        column: x => x.IdOrdenServicio,
                        principalTable: "OrdenesServicio",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DetallesOrdenServicio_ServiciosTaller_IdServicioTaller",
                        column: x => x.IdServicioTaller,
                        principalTable: "ServiciosTaller",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Facturas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrdenServicioId = table.Column<int>(type: "integer", nullable: false),
                    NumeroFactura = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    SubtotalManoObra = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    SubtotalRepuestos = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    TotalImpuestos = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    TotalNeto = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    FechaEmision = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    EstadoPago = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false, defaultValue: "Pendiente")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Facturas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Facturas_OrdenesServicio_OrdenServicioId",
                        column: x => x.OrdenServicioId,
                        principalTable: "OrdenesServicio",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GarantiasServicio",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrdenServicioActualId = table.Column<int>(type: "integer", nullable: false),
                    OrdenServicioOrigenId = table.Column<int>(type: "integer", nullable: false),
                    MotivoFalla = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    ResolucionDictamen = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    FechaReclamo = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GarantiasServicio", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GarantiasServicio_OrdenesServicio_OrdenServicioActualId",
                        column: x => x.OrdenServicioActualId,
                        principalTable: "OrdenesServicio",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GarantiasServicio_OrdenesServicio_OrdenServicioOrigenId",
                        column: x => x.OrdenServicioOrigenId,
                        principalTable: "OrdenesServicio",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HerramientaPrestamos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    HerramientaId = table.Column<int>(type: "integer", nullable: false),
                    UsuarioMecanicoId = table.Column<int>(type: "integer", nullable: false),
                    OrdenServicioId = table.Column<int>(type: "integer", nullable: false),
                    FechaPrestamo = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    FechaDevolucion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HerramientaPrestamos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HerramientaPrestamos_Herramientas_HerramientaId",
                        column: x => x.HerramientaId,
                        principalTable: "Herramientas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HerramientaPrestamos_OrdenesServicio_OrdenServicioId",
                        column: x => x.OrdenServicioId,
                        principalTable: "OrdenesServicio",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HerramientaPrestamos_Usuarios_UsuarioMecanicoId",
                        column: x => x.UsuarioMecanicoId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrdenAseguradoraDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrdenServicioId = table.Column<int>(type: "integer", nullable: false),
                    AseguradoraId = table.Column<int>(type: "integer", nullable: false),
                    NumeroSiniestro = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    MontoMaximoAprobado = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    FechaAutorizacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdenAseguradoraDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrdenAseguradoraDetails_Aseguradoras_AseguradoraId",
                        column: x => x.AseguradoraId,
                        principalTable: "Aseguradoras",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdenAseguradoraDetails_OrdenesServicio_OrdenServicioId",
                        column: x => x.OrdenServicioId,
                        principalTable: "OrdenesServicio",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrdenEstadoHistorial",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrdenServicioId = table.Column<int>(type: "integer", nullable: false),
                    EstadoAnterior = table.Column<string>(type: "text", nullable: false),
                    EstadoNuevo = table.Column<string>(type: "text", nullable: false),
                    FechaCambio = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UsuarioId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdenEstadoHistorial", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrdenEstadoHistorial_OrdenesServicio_OrdenServicioId",
                        column: x => x.OrdenServicioId,
                        principalTable: "OrdenesServicio",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrdenEstadoHistorial_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DescuentosFactura",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdFactura = table.Column<int>(type: "integer", nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Porcentaje = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    MontoDescontado = table.Column<decimal>(type: "numeric(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DescuentosFactura", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DescuentosFactura_Facturas_IdFactura",
                        column: x => x.IdFactura,
                        principalTable: "Facturas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FacturaImpuestos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FacturaId = table.Column<int>(type: "integer", nullable: false),
                    NombreImpuesto = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Porcentaje = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    MontoCalculado = table.Column<decimal>(type: "numeric(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacturaImpuestos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FacturaImpuestos_Facturas_FacturaId",
                        column: x => x.FacturaId,
                        principalTable: "Facturas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FacturaPagos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FacturaId = table.Column<int>(type: "integer", nullable: false),
                    MedioPagoId = table.Column<int>(type: "integer", nullable: false),
                    MontoPagado = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    FechaPago = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    TransaccionReferencia = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false, defaultValue: "")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacturaPagos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FacturaPagos_Facturas_FacturaId",
                        column: x => x.FacturaId,
                        principalTable: "Facturas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FacturaPagos_MediosPago_MedioPagoId",
                        column: x => x.MedioPagoId,
                        principalTable: "MediosPago",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AseguradoraCoberturas_AseguradoraId",
                table: "AseguradoraCoberturas",
                column: "AseguradoraId");

            migrationBuilder.CreateIndex(
                name: "IX_Aseguradoras_RfcONit",
                table: "Aseguradoras",
                column: "RfcONit",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AuditoriaTransacciones_UsuarioId",
                table: "AuditoriaTransacciones",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_BahiaHistorialEstados_IdBahiaServicio",
                table: "BahiaHistorialEstados",
                column: "IdBahiaServicio");

            migrationBuilder.CreateIndex(
                name: "IX_BahiaMecanicoAsignaciones_BahiaServicioId",
                table: "BahiaMecanicoAsignaciones",
                column: "BahiaServicioId");

            migrationBuilder.CreateIndex(
                name: "IX_BahiaMecanicoAsignaciones_UsuarioMecanicoId",
                table: "BahiaMecanicoAsignaciones",
                column: "UsuarioMecanicoId");

            migrationBuilder.CreateIndex(
                name: "IX_CierresContablesDiarios_UsuarioAdministradorId",
                table: "CierresContablesDiarios",
                column: "UsuarioAdministradorId");

            migrationBuilder.CreateIndex(
                name: "IX_CitasTaller_ClienteId",
                table: "CitasTaller",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_CitasTaller_ServicioTallerId",
                table: "CitasTaller",
                column: "ServicioTallerId");

            migrationBuilder.CreateIndex(
                name: "IX_CitasTaller_VehiculoId",
                table: "CitasTaller",
                column: "VehiculoId");

            migrationBuilder.CreateIndex(
                name: "IX_DescuentosFactura_IdFactura",
                table: "DescuentosFactura",
                column: "IdFactura");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesOrdenCompra_IdOrdenCompra",
                table: "DetallesOrdenCompra",
                column: "IdOrdenCompra");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesOrdenCompra_IdRepuesto",
                table: "DetallesOrdenCompra",
                column: "IdRepuesto");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesOrdenRepuesto_OrdenServicioId",
                table: "DetallesOrdenRepuesto",
                column: "OrdenServicioId");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesOrdenRepuesto_RepuestoId",
                table: "DetallesOrdenRepuesto",
                column: "RepuestoId");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesOrdenServicio_IdOrdenServicio",
                table: "DetallesOrdenServicio",
                column: "IdOrdenServicio");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesOrdenServicio_IdServicioTaller",
                table: "DetallesOrdenServicio",
                column: "IdServicioTaller");

            migrationBuilder.CreateIndex(
                name: "IX_FacturaImpuestos_FacturaId",
                table: "FacturaImpuestos",
                column: "FacturaId");

            migrationBuilder.CreateIndex(
                name: "IX_FacturaPagos_FacturaId",
                table: "FacturaPagos",
                column: "FacturaId");

            migrationBuilder.CreateIndex(
                name: "IX_FacturaPagos_MedioPagoId",
                table: "FacturaPagos",
                column: "MedioPagoId");

            migrationBuilder.CreateIndex(
                name: "IX_Facturas_NumeroFactura",
                table: "Facturas",
                column: "NumeroFactura",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Facturas_OrdenServicioId",
                table: "Facturas",
                column: "OrdenServicioId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GarantiasServicio_OrdenServicioActualId",
                table: "GarantiasServicio",
                column: "OrdenServicioActualId");

            migrationBuilder.CreateIndex(
                name: "IX_GarantiasServicio_OrdenServicioOrigenId",
                table: "GarantiasServicio",
                column: "OrdenServicioOrigenId");

            migrationBuilder.CreateIndex(
                name: "IX_HerramientaMantenimientos_HerramientaId",
                table: "HerramientaMantenimientos",
                column: "HerramientaId");

            migrationBuilder.CreateIndex(
                name: "IX_HerramientaPrestamos_HerramientaId",
                table: "HerramientaPrestamos",
                column: "HerramientaId");

            migrationBuilder.CreateIndex(
                name: "IX_HerramientaPrestamos_OrdenServicioId",
                table: "HerramientaPrestamos",
                column: "OrdenServicioId");

            migrationBuilder.CreateIndex(
                name: "IX_HerramientaPrestamos_UsuarioMecanicoId",
                table: "HerramientaPrestamos",
                column: "UsuarioMecanicoId");

            migrationBuilder.CreateIndex(
                name: "IX_Herramientas_CategoriaId",
                table: "Herramientas",
                column: "CategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_Herramientas_CodigoActivo",
                table: "Herramientas",
                column: "CodigoActivo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HistorialesKilometraje_VehiculoId",
                table: "HistorialesKilometraje",
                column: "VehiculoId");

            migrationBuilder.CreateIndex(
                name: "IX_InsumoConsumoHistorial_InsumoTallerId",
                table: "InsumoConsumoHistorial",
                column: "InsumoTallerId");

            migrationBuilder.CreateIndex(
                name: "IX_MecanicoCertificaciones_UsuarioId",
                table: "MecanicoCertificaciones",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_MecanicoEspecialidades_UsuarioId",
                table: "MecanicoEspecialidades",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenAseguradoraDetails_AseguradoraId",
                table: "OrdenAseguradoraDetails",
                column: "AseguradoraId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenAseguradoraDetails_OrdenServicioId",
                table: "OrdenAseguradoraDetails",
                column: "OrdenServicioId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesCompra_CodigoOrden",
                table: "OrdenesCompra",
                column: "CodigoOrden",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesCompra_ProveedorId",
                table: "OrdenesCompra",
                column: "ProveedorId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesServicio_IdCita",
                table: "OrdenesServicio",
                column: "IdCita",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesServicio_IdMecanico",
                table: "OrdenesServicio",
                column: "IdMecanico");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesServicio_IdVehiculo",
                table: "OrdenesServicio",
                column: "IdVehiculo");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesServicio_ServicioTallerId",
                table: "OrdenesServicio",
                column: "ServicioTallerId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenEstadoHistorial_OrdenServicioId",
                table: "OrdenEstadoHistorial",
                column: "OrdenServicioId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenEstadoHistorial_UsuarioId",
                table: "OrdenEstadoHistorial",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_ProveedorContactos_IdProveedor",
                table: "ProveedorContactos",
                column: "IdProveedor");

            migrationBuilder.CreateIndex(
                name: "IX_ProveedorRepuestos_ProveedorId",
                table: "ProveedorRepuestos",
                column: "ProveedorId");

            migrationBuilder.CreateIndex(
                name: "IX_ProveedorRepuestos_RepuestoId",
                table: "ProveedorRepuestos",
                column: "RepuestoId");

            migrationBuilder.CreateIndex(
                name: "IX_Repuestos_Codigo",
                table: "Repuestos",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RepuestoUbicaciones_RepuestoId",
                table: "RepuestoUbicaciones",
                column: "RepuestoId");

            migrationBuilder.CreateIndex(
                name: "IX_TokenBlocklist_TokenHash",
                table: "TokenBlocklist",
                column: "TokenHash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioControlAccesos_UsuarioId",
                table: "UsuarioControlAccesos",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioHorarios_UsuarioId",
                table: "UsuarioHorarios",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Correo",
                table: "Usuarios",
                column: "Correo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vehiculos_IdCliente",
                table: "Vehiculos",
                column: "IdCliente");

            migrationBuilder.CreateIndex(
                name: "IX_Vehiculos_VIN",
                table: "Vehiculos",
                column: "VIN",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AseguradoraCoberturas");

            migrationBuilder.DropTable(
                name: "AuditoriaTransacciones");

            migrationBuilder.DropTable(
                name: "BahiaHistorialEstados");

            migrationBuilder.DropTable(
                name: "BahiaMecanicoAsignaciones");

            migrationBuilder.DropTable(
                name: "CierresContablesDiarios");

            migrationBuilder.DropTable(
                name: "DescuentosFactura");

            migrationBuilder.DropTable(
                name: "DetallesOrdenCompra");

            migrationBuilder.DropTable(
                name: "DetallesOrdenRepuesto");

            migrationBuilder.DropTable(
                name: "DetallesOrdenServicio");

            migrationBuilder.DropTable(
                name: "FacturaImpuestos");

            migrationBuilder.DropTable(
                name: "FacturaPagos");

            migrationBuilder.DropTable(
                name: "GarantiasServicio");

            migrationBuilder.DropTable(
                name: "HerramientaMantenimientos");

            migrationBuilder.DropTable(
                name: "HerramientaPrestamos");

            migrationBuilder.DropTable(
                name: "HistorialesKilometraje");

            migrationBuilder.DropTable(
                name: "InsumoConsumoHistorial");

            migrationBuilder.DropTable(
                name: "MecanicoCertificaciones");

            migrationBuilder.DropTable(
                name: "MecanicoEspecialidades");

            migrationBuilder.DropTable(
                name: "OrdenAseguradoraDetails");

            migrationBuilder.DropTable(
                name: "OrdenEstadoHistorial");

            migrationBuilder.DropTable(
                name: "ProveedorContactos");

            migrationBuilder.DropTable(
                name: "ProveedorRepuestos");

            migrationBuilder.DropTable(
                name: "RepuestoUbicaciones");

            migrationBuilder.DropTable(
                name: "TokenBlocklist");

            migrationBuilder.DropTable(
                name: "UsuarioControlAccesos");

            migrationBuilder.DropTable(
                name: "UsuarioHorarios");

            migrationBuilder.DropTable(
                name: "BahiasServicio");

            migrationBuilder.DropTable(
                name: "OrdenesCompra");

            migrationBuilder.DropTable(
                name: "Facturas");

            migrationBuilder.DropTable(
                name: "MediosPago");

            migrationBuilder.DropTable(
                name: "Herramientas");

            migrationBuilder.DropTable(
                name: "InsumosTaller");

            migrationBuilder.DropTable(
                name: "Aseguradoras");

            migrationBuilder.DropTable(
                name: "Repuestos");

            migrationBuilder.DropTable(
                name: "Proveedor");

            migrationBuilder.DropTable(
                name: "OrdenesServicio");

            migrationBuilder.DropTable(
                name: "HerramientaCategorias");

            migrationBuilder.DropTable(
                name: "CitasTaller");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "ServiciosTaller");

            migrationBuilder.DropTable(
                name: "Vehiculos");

            migrationBuilder.DropTable(
                name: "Clientes");
        }
    }
}
