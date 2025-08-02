using FluentResults;
using TaskManager.Application.Dtos.TaskDto;

namespace TaskManager.Application.UseCase.Tasks.Interfaces;

public interface IUpdateStatusTaskUseCase
{
    public Task<Result> UpdateStatus(UpdateStatusTaskDto request, int id);
}