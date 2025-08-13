using AutoMapper;
using FluentResults;
using Microsoft.Extensions.Logging;
using TaskManager.Application.UseCase.Tasks.Interfaces;
using TaskManager.Domain.Interfaces.ITask;

namespace TaskManager.Application.UseCase.Tasks.UseCases;

public class DeleteTaskUseCase : IDeleteTaskUseCase
{
    private readonly ITaskRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<DeleteTaskUseCase> _logger;

    public DeleteTaskUseCase(ITaskRepository repository, IMapper mapper, ILogger<DeleteTaskUseCase> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result> DeleteTaskAsync(int taskId, int userId)
    {
        try
        {
            _logger.LogInformation("Iniciando deleção da tarefa com ID {TaskId} para o usuário com ID {UserId}", taskId, userId);

            var task = await _repository.GetId(taskId, userId);

            await _repository.DeleteTask(task!, userId);

            _logger.LogInformation("Tarefa com ID {TaskId} deletada com sucesso para o usuário com ID {UserId}", taskId, userId);

            return Result.Ok();
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Erro ao deletar tarefa com ID {TaskId} para o usuário com ID {UserId}", taskId, userId);
            return Result.Fail($"Erro inesperado ao deletar tarefa: {ex.Message}");
        }
        
    }
}
