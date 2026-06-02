using System;

namespace AutoTallerManager.Application.Exceptions;

/// <summary>
/// Excepción de negocio fuertemente tipada que indica la violación de una regla de dominio
/// o de validación de negocio en el caso de uso.
/// </summary>
public class BusinessException : Exception
{
    public BusinessException(string message) : base(message)
    {
    }
}
