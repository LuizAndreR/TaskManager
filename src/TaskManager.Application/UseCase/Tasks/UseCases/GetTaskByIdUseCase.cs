using AutoMapper;
using FluentResults;
using Microsoft.Extensions.Logging;
using TaskManager.Application.Dtos.TaskDto;
using TaskManager.Application.UseCase.Tasks.Interfaces;
using TaskManager.Domain.Interfaces.ITask;

namespace TaskManager.Application.UseCase.Tasks.UseCases;

public class GetTaskByIdUseCase : IGetTaskByIdUseCase
{
    private readonly ITaskRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetTaskByIdUseCase> _logger;

    public GetTaskByIdUseCase(ITaskRepository repository, IMapper mapper, ILogger<GetTaskByIdUseCase> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }
    
    public async Task<Result<GetTaskDto>> GetTaskByIdAsync(int id, int userId)
    {
        try
        {
            _logger.LogInformation("Iniciando busca da tarefa com ID {Id} para o usuário com ID {UserId}", id, userId);

            var tarefa = await _repository.GetId(id, userId);

            if (tarefa == null)
            {
                _logger.LogWarning("Nenhuma tarefa encontrada com ID {Id} para o usuário com ID {UserId}", id, userId);
                return Result.Fail<GetTaskDto>("Nenhuma tarefa encontrada com id");
            }

            var tarefaDto = _mapper.Map<GetTaskDto>(tarefa);

            _logger.LogInformation("Tarefa com ID {Id} encontrada com sucesso para o usuário com ID {UserId}", id, userId);

            return Result.Ok(tarefaDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao buscar tarefa com ID {Id} para o usuário com ID {UserId}", id, userId);
            return Result.Fail<GetTaskDto>($"Erro interno inesperado: {ex.Message}");
        }
    }
}