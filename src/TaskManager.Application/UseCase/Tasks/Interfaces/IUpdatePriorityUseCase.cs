using FluentResults;
using TaskManager.Application.Dtos.TaskDto;

namespace TaskManager.Application.UseCase.Tasks.Interfaces;

public interface IUpdatePriorityUseCase
{
    public Task<Result> UpdatePriorityAsync(UpdatePriorityDto request, int taskId, int userId);
}
