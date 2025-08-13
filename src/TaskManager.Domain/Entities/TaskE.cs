namespace TaskManager.Domain.Entities;

public class TaskE
{
    public int Id { get; set; }
    public string Title { get; set; } = "Teste";
    public string? Descriptions { get; set; }
    public string Priority { get; set; } = "Media";
    public string Status { get; set; } = "Ativo";
    public DateTime DateCreated { get; set; } = DateTime.UtcNow.Date;
    public int UsuarioId { get; set; }
    public required Usuario Usuario { get; set; }
}
