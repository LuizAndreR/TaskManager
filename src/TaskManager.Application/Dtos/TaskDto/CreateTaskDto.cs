namespace TaskManager.Application.Dtos.TaskDto;

public class CreateTaskDto
{
    public required string Title { get; set; }
    public string? Descriptions { get; set; }
    public required string Priority { get; set; }
    public required string Status { get; set; }
}
