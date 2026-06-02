using System;

namespace AutoTallerManager.Domain.Entities;

public class OrdenServicio
{
    public int Id { get; private set; }
    public int VehiculoId { get; private set; }
    public string Estado { get; private set; } = "Iniciada"; // Iniciada, En Progreso, Finalizada
    public DateTime FechaIngreso { get; private set; } = DateTime.UtcNow;
    public string DescripcionProblema { get; private set; } = string.Empty;
    public decimal CostoEstimado { get; private set; } = 0.0m;
    public string? DiagnosticoMecanico { get; private set; }
    public DateTime? FechaEntrega { get; private set; }
    public decimal? CostoTotal { get; private set; }
    public int? MecanicoId { get; private set; }

    // Propiedad de navegación hacia el vehículo
    public virtual Vehiculo Vehiculo { get; set; } = null!;

    // Constructor vacío requerido por Entity Framework Core
    public OrdenServicio()
    {
    }

    // Constructor de negocio para garantizar la existencia de objetos válidos en memoria
    public OrdenServicio(int vehiculoId, string descripcionProblema, decimal costoEstimado)
    {
        if (vehiculoId <= 0)
            throw new ArgumentException("La orden debe estar asociada a un vehículo válido.", nameof(vehiculoId));

        if (string.IsNullOrWhiteSpace(descripcionProblema))
            throw new ArgumentException("La descripción del problema es obligatoria.", nameof(descripcionProblema));

        if (costoEstimado < 0)
            throw new ArgumentException("El costo estimado no puede ser negativo.", nameof(costoEstimado));

        VehiculoId = vehiculoId;
        DescripcionProblema = descripcionProblema.Trim();
        CostoEstimado = costoEstimado;
        Estado = "Iniciada";
        FechaIngreso = DateTime.UtcNow;
    }

    /// <summary>
    /// Asigna un mecánico responsable a la orden de servicio.
    /// Valida que la orden no se encuentre cerrada o finalizada.
    /// </summary>
    public void AsignarMecanico(int mecanicoId)
    {
        if (mecanicoId <= 0)
            throw new ArgumentException("El identificador del mecánico debe ser válido.", nameof(mecanicoId));

        if (Estado == "Finalizada")
            throw new InvalidOperationException("No se puede asignar un mecánico a una orden que ya está finalizada y facturada.");

        MecanicoId = mecanicoId;
        
        // Al asignar el mecánico, si la orden estaba Iniciada, pasa automáticamente a En Progreso
        if (Estado == "Iniciada")
        {
            CambiarEstado("En Progreso");
        }
    }

    /// <summary>
    /// Cambia de manera segura y lógica el estado de la orden respetando las reglas de negocio.
    /// </summary>
    public void CambiarEstado(string nuevoEstado)
    {
        if (string.IsNullOrWhiteSpace(nuevoEstado))
            throw new ArgumentException("El nuevo estado no puede estar vacío.", nameof(nuevoEstado));

        var nuevoEstadoLimpio = nuevoEstado.Trim();

        if (nuevoEstadoLimpio != "Iniciada" && nuevoEstadoLimpio != "En Progreso" && nuevoEstadoLimpio != "Finalizada")
            throw new InvalidOperationException($"El estado '{nuevoEstadoLimpio}' no es un estado válido del ciclo de vida de la orden.");

        if (Estado == "Finalizada" && nuevoEstadoLimpio != "Finalizada")
            throw new InvalidOperationException("Una orden finalizada y cerrada no puede revertirse a un estado anterior.");

        Estado = nuevoEstadoLimpio;
    }

    /// <summary>
    /// Calcula de forma segura el costo total validando que no se registren montos negativos.
    /// </summary>
    public void CalcularTotal(decimal costoManoObra, decimal costoRepuestos)
    {
        if (costoManoObra < 0)
            throw new ArgumentException("El costo de mano de obra no puede ser negativo.", nameof(costoManoObra));

        if (costoRepuestos < 0)
            throw new ArgumentException("El costo de repuestos no puede ser negativo.", nameof(costoRepuestos));

        CostoTotal = costoManoObra + costoRepuestos;
    }

    /// <summary>
    /// Registra el diagnóstico mecánico oficial de la orden.
    /// </summary>
    public void RegistrarDiagnostico(string diagnostico)
    {
        if (string.IsNullOrWhiteSpace(diagnostico))
            throw new ArgumentException("El diagnóstico mecánico no puede estar vacío.", nameof(diagnostico));

        if (Estado == "Finalizada")
            throw new InvalidOperationException("No se puede registrar un diagnóstico en una orden finalizada.");

        DiagnosticoMecanico = diagnostico.Trim();
    }

    /// <summary>
    /// Finaliza, fecha y cierra de forma definitiva la orden de servicio.
    /// </summary>
    public void FinalizarOrden()
    {
        CambiarEstado("Finalizada");
        FechaEntrega = DateTime.UtcNow;
    }

    /// <summary>
    /// Método autónomo de validación de consistencia.
    /// </summary>
    public void Validar()
    {
        if (VehiculoId <= 0)
            throw new InvalidOperationException("La orden debe estar asociada a un vehículo válido.");

        if (string.IsNullOrWhiteSpace(DescripcionProblema))
            throw new InvalidOperationException("La descripción del problema es obligatoria.");

        if (CostoEstimado < 0)
            throw new InvalidOperationException("El costo estimado no puede ser negativo.");

        if (CostoTotal.HasValue && CostoTotal.Value < 0)
            throw new InvalidOperationException("El costo total calculado no puede ser negativo.");
    }
}
