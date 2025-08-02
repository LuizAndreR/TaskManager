using FluentResults;
using TaskManager.Application.Dtos.TaskDto;

namespace TaskManager.Application.UseCase.Tasks.Interfaces;

public interface IGetTaskByIdUseCase
{
    public Task<Result<GetTaskDto>> GetTaskByIdAsync(int id, int userId);
}