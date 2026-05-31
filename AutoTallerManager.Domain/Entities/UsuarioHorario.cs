namespace AutoTallerManager.Domain.Entities;

public class UsuarioHorario
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public DayOfWeek DiaSemana { get; set; }
    public TimeSpan HoraInicio { get; set; }
    public TimeSpan HoraFin { get; set; }

    public virtual Usuario? Usuario { get; set; }
}
