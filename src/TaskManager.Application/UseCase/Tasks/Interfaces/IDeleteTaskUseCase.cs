using FluentResults;

namespace TaskManager.Application.UseCase.Tasks.Interfaces;

public interface IDeleteTaskUseCase
{
    public Task<Result> DeleteTaskAsync(int taskId, int userId);
}
