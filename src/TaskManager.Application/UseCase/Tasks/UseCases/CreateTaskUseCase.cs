using AutoMapper;
using FluentResults;
using FluentValidation;
using Microsoft.Extensions.Logging;
using TaskManager.Application.Dtos.TaskDto;
using TaskManager.Application.UseCase.Tasks.Interfaces;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Interfaces.ITask;

namespace TaskManager.Application.UseCase.Tasks.UseCases;

public class CreateTaskUseCase : ICreateTaskUseCase
{
    private readonly ITaskRepository _repository;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateTaskDto> _validator;
    
    private readonly ILogger<CreateTaskUseCase> _logger;

    public CreateTaskUseCase(ITaskRepository repository, IMapper mapper, IValidator<CreateTaskDto> validator, ILogger<CreateTaskUseCase> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
        _validator = validator;
    }
    
    public async Task<Result<GetTaskDto>> CreateTaskAsync(CreateTaskDto createDto, int userId)
    {
        try
        {   
            _logger.LogInformation("Iniciando validação para criação de nova tarefa com título: {Titulo}", createDto.Title);

            var validatorResult = _validator.Validate(createDto);

            if (!validatorResult.IsValid)
            {
                _logger.LogWarning("Validação falhou ao criar tarefa: {Titulo}. Erros: {Erros}",createDto.Title, string.Join(", ", validatorResult.Errors.Select(e => e.ErrorMessage)));

                var result = Result.Fail<GetTaskDto>("Erro de validação");

                foreach (var erro in validatorResult.Errors)
                {
                    result.WithError(erro.ErrorMessage);
                }

                return result;
            }

            _logger.LogInformation("Validação concluída com sucesso para a tarefa: {Titulo}", createDto.Title);

            var taskE = _mapper.Map<TaskE>(createDto);
            taskE.UsuarioId = userId;

            _logger.LogInformation("Tarefa mapeada para entidade com prioridade: {Prioridade} e status: {Status}",taskE.Priority, taskE.Status);

            taskE = await _repository.CreateTask(taskE);

            _logger.LogInformation("Tarefa criada com sucesso no repositório. ID: {Id}", taskE.Id);

            var taskGet = _mapper.Map<GetTaskDto>(taskE);

            return Result.Ok(taskGet);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao criar uma nova tarefa com título: {Titulo}", createDto.Title);
            return Result.Fail<GetTaskDto>($"Erro interno inesperado: {ex.Message}");
        }
    }
}