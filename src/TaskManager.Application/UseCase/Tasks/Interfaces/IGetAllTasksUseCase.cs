using FluentResults;
using TaskManager.Application.Dtos.TaskDto;

namespace TaskManager.Application.UseCase.Tasks.Interfaces;

public interface IGetAllTasksUseCase
{
    public Task<Result<IEnumerable<GetTaskDto>>>  GetAllTasksAsync(int userId);
}