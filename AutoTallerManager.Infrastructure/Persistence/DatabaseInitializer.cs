using AutoTallerManager.Domain.Entities;
using AutoTallerManager.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AutoTallerManager.Infrastructure.Persistence
{
    /// <summary>
    /// Se encarga de inicializar la base de datos de forma segura, ejecutando migraciones pendientes 
    /// y realizando la siembra (seeding) de datos de catálogo base de manera idempotente.
    /// </summary>
    public static class DatabaseInitializer
    {
        public static async Task InitializeDatabaseAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var services = scope.ServiceProvider;
            var logger = services.GetRequiredService<ILogger<AutoTallerDbContext>>();

            try
            {
                var context = services.GetRequiredService<AutoTallerDbContext>();
                var passwordHasher = services.GetRequiredService<IPasswordHasher>();

                logger.LogInformation("--> [MIGRACIÓN] Iniciando la aplicación automática de migraciones pendientes en PostgreSQL...");
                await context.Database.MigrateAsync();
                logger.LogInformation("--> [MIGRACIÓN] Migraciones aplicadas o validadas de forma exitosa en la base de datos.");

                // Ejecución blindada e idempotente del sembrado de datos base
                await SeedDataAsync(context, passwordHasher, logger);
            }
            catch (Exception ex)
            {
                logger.LogCritical(ex, "--> [FALLO CRÍTICO] Error insalvable durante el arranque, migración o siembra de la base de datos.");
                throw; // Se propaga la excepción para provocar un fallo controlado en el arranque del servicio
            }
        }

        private static async Task SeedDataAsync(AutoTallerDbContext context, IPasswordHasher passwordHasher, ILogger<AutoTallerDbContext> logger)
        {
            logger.LogInformation("--> [SEEDING] Iniciando el proceso de sembrado de datos de catálogo base...");

            // 1. Validación de Roles Inmutables
            // Dado que en el diseño actual los roles se almacenan como cadenas de texto ("Admin", "Mecanico", "Recepcionista", "Cliente")
            // y no existe una tabla física 'Roles', validamos de forma conceptual la presencia de usuarios con dichos roles.
            await VerifyImmutableRolesAsync(context, logger);

            // 2. Sembrado de Usuario Administrador inicial
            await SeedAdminUserAsync(context, passwordHasher, logger);

            // 3. Sembrado de Catálogo de Servicios
            await SeedServicesCatalogAsync(context, logger);

            // 4. Sembrado de Catálogo de Repuestos
            await SeedSparePartsCatalogAsync(context, logger);

            // 5. Sembrado de Medios de Pago
            await SeedMediosPagoAsync(context, logger);

            logger.LogInformation("--> [SEEDING] Proceso de sembrado de datos de catálogo base finalizado correctamente.");
        }

        private static async Task VerifyImmutableRolesAsync(AutoTallerDbContext context, ILogger<AutoTallerDbContext> logger)
        {
            logger.LogInformation("--> [SEEDING] Validando roles inmutables en el sistema...");

            // Definición de roles inmutables de negocio
            var immutableRoles = new[] { "Admin", "Mecanico", "Recepcionista", "Cliente" };

            // Realizamos la validación condicional asíncrona simulando una verificación de la persistencia
            foreach (var rol in immutableRoles)
            {
                // Como ejemplo de validación, registramos cuántos usuarios existen asignados a este rol de forma asíncrona
                var existsWithRole = await context.Usuarios.AnyAsync(u => u.Rol == rol);
                logger.LogInformation("--> [SEEDING] Rol inmutable '{Rol}' verificado. ¿Tiene usuarios asignados?: {Exists}", rol, existsWithRole);
            }
        }

        private static async Task SeedAdminUserAsync(AutoTallerDbContext context, IPasswordHasher passwordHasher, ILogger<AutoTallerDbContext> logger)
        {
            const string adminEmail = "admin@autotaller.com";

            // Validación condicional asíncrona para garantizar la idempotencia
            var adminExists = await context.Usuarios.AnyAsync(u => u.Correo == adminEmail);

            if (!adminExists)
            {
                logger.LogInformation("--> [SEEDING] Usuario administrador no encontrado. Creando credenciales por defecto...");

                var adminUser = new Usuario
                {
                    Nombre = "Administrador del Sistema",
                    Correo = adminEmail,
                    Rol = "Admin",
                    Activo = true
                };

                // Hasheo agnóstico a través de la interfaz abstracta IPasswordHasher
                adminUser.ContrasenaHash = passwordHasher.HashPassword("Admin123*");

                await context.Usuarios.AddAsync(adminUser);
                await context.SaveChangesAsync();

                logger.LogInformation("--> [SEEDING] Usuario administrador sembrado exitosamente: {Email} / Admin123*", adminEmail);
            }
            else
            {
                logger.LogInformation("--> [SEEDING] El usuario administrador ({Email}) ya existe. Se omite el sembrado.", adminEmail);
            }
        }

        private static async Task SeedServicesCatalogAsync(AutoTallerDbContext context, ILogger<AutoTallerDbContext> logger)
        {
            // Validación condicional asíncrona antes de insertar para garantizar la idempotencia
            var hasServices = await context.ServiciosTaller.AnyAsync();

            if (!hasServices)
            {
                logger.LogInformation("--> [SEEDING] Catálogo de servicios vacío. Sembrando catálogo base de servicios del taller...");

                var servicios = new List<ServicioTaller>
                {
                    new ServicioTaller 
                    { 
                        Nombre = "Cambio de Aceite y Filtro", 
                        Descripcion = "Servicio preventivo básico que incluye cambio de aceite de motor y filtro de aceite.", 
                        TarifaBaseManoObra = 25.00m, 
                        Activo = true 
                    },
                    new ServicioTaller 
                    { 
                        Nombre = "Alineación y Balanceo", 
                        Descripcion = "Alineación de las cuatro ruedas y balanceo dinámico para evitar vibraciones en carretera.", 
                        TarifaBaseManoObra = 35.00m, 
                        Activo = true 
                    },
                    new ServicioTaller 
                    { 
                        Nombre = "Revisión del Sistema de Frenos", 
                        Descripcion = "Inspección detallada de pastillas, discos, caliper y líquido de frenos con informe técnico.", 
                        TarifaBaseManoObra = 15.00m, 
                        Activo = true 
                    },
                    new ServicioTaller 
                    { 
                        Nombre = "Diagnóstico Computarizado de Motor", 
                        Descripcion = "Escaneo electrónico integral mediante OBD-II de la computadora del vehículo para detectar códigos de falla.", 
                        TarifaBaseManoObra = 40.00m, 
                        Activo = true 
                    },
                    new ServicioTaller 
                    { 
                        Nombre = "Mantenimiento General Preventivo", 
                        Descripcion = "Revisión integral y puesta a punto de los principales sistemas mecánicos, eléctricos y fluidos del vehículo.", 
                        TarifaBaseManoObra = 100.00m, 
                        Activo = true 
                    }
                };

                await context.ServiciosTaller.AddRangeAsync(servicios);
                await context.SaveChangesAsync();

                logger.LogInformation("--> [SEEDING] Catálogo base de servicios sembrado exitosamente ({Count} servicios).", servicios.Count);
            }
            else
            {
                logger.LogInformation("--> [SEEDING] Catálogo de servicios ya cuenta con registros. Se omite el sembrado.");
            }
        }

        private static async Task SeedMediosPagoAsync(AutoTallerDbContext context, ILogger<AutoTallerDbContext> logger)
        {
            var hasMediosPago = await context.MediosPago.AnyAsync();

            if (!hasMediosPago)
            {
                logger.LogInformation("--> [SEEDING] Tabla de medios de pago vacía. Sembrando catálogo base...");

                var medios = new List<MedioPago>
                {
                    new MedioPago { Nombre = "Efectivo", PermiteCuotas = false, Activo = true },
                    new MedioPago { Nombre = "Tarjeta de Crédito", PermiteCuotas = true, Activo = true },
                    new MedioPago { Nombre = "Tarjeta de Débito", PermiteCuotas = false, Activo = true },
                    new MedioPago { Nombre = "Transferencia Bancaria", PermiteCuotas = false, Activo = true }
                };

                await context.MediosPago.AddRangeAsync(medios);
                await context.SaveChangesAsync();

                logger.LogInformation("--> [SEEDING] Medios de pago sembrados exitosamente ({Count} registros).", medios.Count);
            }
            else
            {
                logger.LogInformation("--> [SEEDING] Medios de pago ya cuentan con registros. Se omite el sembrado.");
            }
        }

        private static async Task SeedSparePartsCatalogAsync(AutoTallerDbContext context, ILogger<AutoTallerDbContext> logger)
        {
            // Validación condicional asíncrona antes de insertar para garantizar la idempotencia
            var hasSpareParts = await context.Repuestos.AnyAsync();

            if (!hasSpareParts)
            {
                logger.LogInformation("--> [SEEDING] Catálogo de repuestos vacío. Sembrando catálogo base de repuestos e insumos...");

                var repuestos = new List<Repuesto>
                {
                    new Repuesto 
                    { 
                        Codigo = "ACEITE-10W40", 
                        Descripcion = "Aceite de Motor Sintético 10W40 (Presentación de 1 Litro)", 
                        Stock = 50, 
                        PrecioUnitario = 12.50m, 
                        Activo = true 
                    },
                    new Repuesto 
                    { 
                        Codigo = "FILTRO-ACEITE", 
                        Descripcion = "Filtro de Aceite de Motor Metálico Estándar", 
                        Stock = 30, 
                        PrecioUnitario = 8.00m, 
                        Activo = true 
                    },
                    new Repuesto 
                    { 
                        Codigo = "PAST-FRE-DEL", 
                        Descripcion = "Pastillas de Freno Cerámicas Delanteras (Par)", 
                        Stock = 15, 
                        PrecioUnitario = 45.00m, 
                        Activo = true 
                    },
                    new Repuesto 
                    { 
                        Codigo = "BUJIA-IRIDIUM", 
                        Descripcion = "Bujía de Encendido Iridium de Alta Duración", 
                        Stock = 100, 
                        PrecioUnitario = 7.50m, 
                        Activo = true 
                    },
                    new Repuesto 
                    { 
                        Codigo = "FILTRO-AIRE", 
                        Descripcion = "Filtro de Aire de Motor de Alto Flujo Lavable", 
                        Stock = 20, 
                        PrecioUnitario = 18.00m, 
                        Activo = true 
                    }
                };

                await context.Repuestos.AddRangeAsync(repuestos);
                await context.SaveChangesAsync();

                logger.LogInformation("--> [SEEDING] Catálogo base de repuestos sembrado exitosamente ({Count} repuestos).", repuestos.Count);
            }
            else
            {
                logger.LogInformation("--> [SEEDING] Catálogo de repuestos ya cuenta con registros. Se omite el sembrado.");
            }
        }
    }
}
