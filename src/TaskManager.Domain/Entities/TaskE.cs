namespace TaskManager.Domain.Entities;

public class TaskE
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public string? Descriptions { get; set; }
    public required string Priority { get; set; }
    public required string Status { get; set; }
    public DateTime DateCreated { get; set; } = DateTime.UtcNow.Date;
    public int UsuarioId { get; set; }
    public required Usuario Usuario { get; set; }
}
