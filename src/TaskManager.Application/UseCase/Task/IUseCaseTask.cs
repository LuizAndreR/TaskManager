using FluentResults;
using TaskManager.Application.Dtos.TaskDto;

namespace TaskManager.Application.UseCase.Task;

public interface IUseCaseTask
{
    public Task<Result<IEnumerable<GetTaskDto>>> BuscaTasksbyIdUserAsync(int userId);
    public Task<Result<GetTaskDto>> BuscaTaskbyIdAsync(int id, int userId);
    public Task<Result<GetTaskDto>> CreateTaskAsync(CreateTaskDto createDto, int userId);
}
