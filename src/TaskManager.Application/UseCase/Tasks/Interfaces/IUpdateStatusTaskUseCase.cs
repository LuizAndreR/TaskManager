using FluentResults;
using TaskManager.Application.Dtos.TaskDto;

namespace TaskManager.Application.UseCase.Tasks.Interfaces;

public interface IUpdateStatusTaskUseCase
{
    public Task<Result> UpdateStatusAsync(UpdateStatusTaskDto request, int taskId, int userId);
}