using AutoMapper;
using FluentResults;
using Microsoft.Extensions.Logging;
using TaskManager.Application.Dtos.TaskDto;
using TaskManager.Application.UseCase.Tasks.Interfaces;
using TaskManager.Domain.Interfaces.ITask;

namespace TaskManager.Application.UseCase.Tasks.UseCases;

public class GetAllTasksUseCase : IGetAllTasksUseCase
{
    private readonly ITaskRepository _repository;
    private readonly IMapper _mapper;
    
    private readonly ILogger<GetAllTasksUseCase> _logger;

    public GetAllTasksUseCase(ITaskRepository repository, IMapper mapper, ILogger<GetAllTasksUseCase> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }
    
    public async Task<Result<IEnumerable<GetTaskDto>>> GetAllTasksAsync(int userId)
    {
        try
        {
            _logger.LogInformation("Iniciando busca de tarefas para o usuário com ID {UserId}", userId);

            var tarefas = await _repository.GetByUserIdAsync(userId);

            if (!tarefas.Any())
            {
                _logger.LogWarning("Nenhuma tarefa encontrada para o usuário com ID {UserId}", userId);
                return Result.Fail<IEnumerable<GetTaskDto>>("Nenhuma tarefa encontrada para o usuário.");
            }

            var tarefasDto = _mapper.Map<IEnumerable<GetTaskDto>>(tarefas);

            IEnumerable<GetTaskDto> getTaskDtos = tarefasDto as GetTaskDto[] ?? tarefasDto.ToArray();
            _logger.LogInformation("{Quantidade} tarefa(s) encontrada(s) para o usuário com ID {UserId}", getTaskDtos.Count(), userId);

            return Result.Ok(getTaskDtos);

        }catch(Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao busca tarefas usuário: {Id}", userId);
            return Result.Fail($"Erro interno inesperado: {ex.Message}");
        }
    }
}