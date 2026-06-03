namespace AutoTallerManager.Application.Interfaces
{
    /// <summary>
    /// Puerto agnóstico que define la abstracción para el hasheo y verificación de contraseñas, 
    /// aislando el core de negocio de dependencias concretas de infraestructura o frameworks de identidad.
    /// </summary>
    public interface IPasswordHasher
    {
        /// <summary>
        /// Genera el hash criptográfico para una contraseña en texto plano.
        /// </summary>
        /// <param name="password">Contraseña en texto plano a hashear.</param>
        /// <returns>Contraseña hasheada segura.</returns>
        string HashPassword(string password);

        /// <summary>
        /// Verifica si una contraseña en texto plano coincide con el hash criptográfico previamente almacenado.
        /// </summary>
        /// <param name="hashedPassword">Contraseña hasheada almacenada.</param>
        /// <param name="providedPassword">Contraseña en texto plano proporcionada para verificación.</param>
        /// <returns>True si coinciden, False en caso contrario.</returns>
        bool VerifyPassword(string hashedPassword, string providedPassword);
    }
}
