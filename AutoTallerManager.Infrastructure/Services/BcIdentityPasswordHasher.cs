using AutoTallerManager.Application.Interfaces;
using AutoTallerManager.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System;

namespace AutoTallerManager.Infrastructure.Services
{
    /// <summary>
    /// Adaptador concreto que implementa el puerto IPasswordHasher encapsulando internamente 
    /// la implementación nativa y altamente segura PasswordHasher de ASP.NET Core Identity.
    /// Esto aísla por completo la capa de aplicación sin alterar los hashes ya guardados.
    /// </summary>
    public class BcIdentityPasswordHasher : IPasswordHasher
    {
        private readonly PasswordHasher<Usuario> _identityHasher;

        public BcIdentityPasswordHasher()
        {
            _identityHasher = new PasswordHasher<Usuario>();
        }

        public string HashPassword(string password)
        {
            if (password == null)
                throw new ArgumentNullException(nameof(password));

            // Satisfacemos el tipo genérico de ASP.NET Identity con un usuario de valor plano/dummy
            var dummyUser = new Usuario();
            return _identityHasher.HashPassword(dummyUser, password);
        }

        public bool VerifyPassword(string hashedPassword, string providedPassword)
        {
            if (hashedPassword == null)
                throw new ArgumentNullException(nameof(hashedPassword));
            if (providedPassword == null)
                throw new ArgumentNullException(nameof(providedPassword));

            var dummyUser = new Usuario();
            var result = _identityHasher.VerifyHashedPassword(dummyUser, hashedPassword, providedPassword);
            
            return result == PasswordVerificationResult.Success || result == PasswordVerificationResult.SuccessRehashNeeded;
        }
    }
}
