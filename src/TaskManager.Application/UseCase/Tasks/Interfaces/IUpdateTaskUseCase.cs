using FluentResults;
using TaskManager.Application.Dtos.TaskDto;

namespace TaskManager.Application.UseCase.Tasks.Interfaces;

public interface IUpdateTaskUseCase
{
    public Task<Result<GetTaskDto>> UpdateTaskAsync(CreateTaskDto editDto, int idTask, int userId);
}