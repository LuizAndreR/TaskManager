using FluentResults;
using TaskManager.Application.Dtos.TaskDto;

namespace TaskManager.Application.UseCase.Tasks.Interfaces;

public interface ICreateTaskUseCase
{
    public Task<Result<GetTaskDto>> CreateTaskAsync(CreateTaskDto createDto, int userId);
}