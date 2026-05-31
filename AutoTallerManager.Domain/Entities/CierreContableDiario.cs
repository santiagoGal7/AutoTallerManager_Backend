namespace AutoTallerManager.Domain.Entities;

public class CierreContableDiario
{
    public int Id { get; set; }
    public DateTime FechaCierre { get; set; }
    public int UsuarioAdministradorId { get; set; } // Quién realiza el arqueo (FK a Usuario)
    public decimal TotalEsperadoSistema { get; set; } // Sumatoria automática de FacturaPago del día
    public decimal TotalRealFisico { get; set; } // Lo que se contó físicamente en caja
    public decimal Diferencia { get; set; } // TotalRealFisico - TotalEsperadoSistema
    public string Observaciones { get; set; } = string.Empty;

    public virtual Usuario? UsuarioAdministrador { get; set; }
}
