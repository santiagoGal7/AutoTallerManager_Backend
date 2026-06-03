namespace AutoTallerManager.Application.DTOs;

public class ReclamoGarantiaDto
{
    public int OrdenOriginalId { get; set; }
    public string MotivoFalla { get; set; } = string.Empty;
    public string ResolucionDictamen { get; set; } = string.Empty;
}
