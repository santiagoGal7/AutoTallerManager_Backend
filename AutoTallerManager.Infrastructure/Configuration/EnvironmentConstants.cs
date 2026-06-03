using System;
using System.IO;
using System.Text.Json;

namespace AutoTallerManager.Infrastructure.Configuration;

public static class EnvironmentConstants
{
    // Claves de variables de entorno
    public const string JwtSecretEnvVar = "JWT_SECRET_KEY";
    public const string DbConnectionEnvVar = "DB_CONNECTION_STRING";

    // Valores por defecto seguros para desarrollo local
    private const string DefaultJwtSecret = "Development_Safe_Fallback_Secret_Key_2026_Min_32_Chars!";
    private const string DefaultConnectionString = "Host=localhost;Database=autotaller;Username=postgres;Password=postgres";

    /// <summary>
    /// Retorna la llave secreta para JWT obtenida desde las variables de entorno, o el fallback seguro en desarrollo.
    /// </summary>
    public static string JwtSecretKey =>
        Environment.GetEnvironmentVariable(JwtSecretEnvVar) ?? DefaultJwtSecret;

    /// <summary>
    /// Retorna la cadena de conexión a la base de datos obtenida desde las variables de entorno, de appsettings.json o del fallback seguro.
    /// </summary>
    public static string ConnectionString
    {
        get
        {
            // 1. Intentar obtener desde variable de entorno
            var envValue = Environment.GetEnvironmentVariable(DbConnectionEnvVar);
            if (!string.IsNullOrEmpty(envValue))
            {
                return envValue;
            }

            // 2. Intentar obtener desde appsettings.json
            try
            {
                var rootDir = Directory.GetCurrentDirectory();
                var jsonPath = Path.Combine(rootDir, "appsettings.json");
                if (!File.Exists(jsonPath))
                {
                    jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
                }

                if (File.Exists(jsonPath))
                {
                    var jsonString = File.ReadAllText(jsonPath);
                    using var doc = JsonDocument.Parse(jsonString);
                    if (doc.RootElement.TryGetProperty("ConnectionStrings", out var connStrings) &&
                        connStrings.TryGetProperty("PostgresConnection", out var postgresConn))
                    {
                        var connStr = postgresConn.GetString();
                        if (!string.IsNullOrEmpty(connStr) && connStr != "YOUR_DATABASE_CONNECTION_STRING_HERE")
                        {
                            return connStr;
                        }
                    }
                }
            }
            catch
            {
                // Fallback silencioso
            }

            // 3. Fallback seguro de desarrollo local
            return DefaultConnectionString;
        }
    }

    /// <summary>
    /// Método utilitario para cargar un archivo .env si existe en la raíz del proyecto (para desarrollo local).
    /// </summary>
    public static void LoadEnvFile()
    {
        try
        {
            var rootDir = Directory.GetCurrentDirectory();
            var envPath = Path.Combine(rootDir, ".env");

            // Si no existe, buscar en el directorio base de la aplicación (para cuando se ejecuta desde bin/)
            if (!File.Exists(envPath))
            {
                envPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ".env");
            }

            // Si sigue sin existir, intentar buscar un nivel arriba (raíz de la solución)
            if (!File.Exists(envPath))
            {
                var parent = Directory.GetParent(rootDir);
                if (parent != null)
                {
                    envPath = Path.Combine(parent.FullName, ".env");
                }
            }

            if (File.Exists(envPath))
            {
                Console.WriteLine($"--> Cargando variables de entorno desde el archivo: {envPath}");
                var lines = File.ReadAllLines(envPath);
                foreach (var line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                        continue;

                    var parts = line.Split('=', 2);
                    if (parts.Length == 2)
                    {
                        var key = parts[0].Trim();
                        var value = parts[1].Trim().Trim('"', '\'');
                        if (!string.IsNullOrEmpty(key))
                        {
                            // Solo establece si la variable de entorno real no está ya definida por el SO
                            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable(key)))
                            {
                                Environment.SetEnvironmentVariable(key, value);
                            }
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"--> Advertencia al cargar el archivo .env: {ex.Message}");
        }
    }
}
