using System;
using System.IO;

namespace AutoTallerManager.Infrastructure.Configuration;

public static class EnvironmentConstants
{
    // Claves de variables de entorno
    public const string JwtSecretEnvVar = "JWT_SECRET_KEY";
    public const string DbConnectionEnvVar = "DB_CONNECTION_STRING";

    // Valores por defecto seguros para desarrollo local
    private const string DefaultJwtSecret = "Development_Safe_Fallback_Secret_Key_2026_Min_32_Chars!";
    private const string DefaultConnectionString = "Host=aws-1-us-east-2.pooler.supabase.com;Port=5432;Database=postgres;Username=postgres.jtgbzvldxcqdwchdxxrz;Password=256752984678;Timeout=30;CommandTimeout=30";

    /// <summary>
    /// Retorna la llave secreta para JWT obtenida desde las variables de entorno, o el fallback seguro en desarrollo.
    /// </summary>
    public static string JwtSecretKey =>
        Environment.GetEnvironmentVariable(JwtSecretEnvVar) ?? DefaultJwtSecret;

    /// <summary>
    /// Retorna la cadena de conexión a la base de datos obtenida desde las variables de entorno, o el fallback seguro en desarrollo.
    /// </summary>
    public static string ConnectionString =>
        Environment.GetEnvironmentVariable(DbConnectionEnvVar) ?? DefaultConnectionString;

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
