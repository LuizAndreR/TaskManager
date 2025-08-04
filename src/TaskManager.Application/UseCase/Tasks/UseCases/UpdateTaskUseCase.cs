using AutoMapper;
using FluentResults;
using FluentValidation;
using Microsoft.Extensions.Logging;
using TaskManager.Application.Dtos.TaskDto;
using TaskManager.Application.UseCase.Tasks.Interfaces;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Interfaces.ITask;

namespace TaskManager.Application.UseCase.Tasks.UseCases;

public class UpdateTaskUseCase : IUpdateTaskUseCase
{
    private readonly ITaskRepository _repository;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateTaskDto> _validator;
    
    private readonly ILogger<UpdateTaskUseCase> _logger;

    public UpdateTaskUseCase(ITaskRepository repository, IMapper mapper, IValidator<CreateTaskDto> validator, ILogger<UpdateTaskUseCase> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
        _validator = validator;
    }
    
    public async Task<Result<GetTaskDto>> UpdateTaskAsync(CreateTaskDto editDto, int idTask, int userId)
    {
        try
        {
            _logger.LogInformation("Iniciando validação para edição da tarefa com ID: {Id} e título: {Titulo}", idTask, editDto.Title);

            var validatorResult = _validator.Validate(editDto);

            if (!validatorResult.IsValid)
            {
                _logger.LogWarning("Validação falhou ao editar tarefa com ID: {Id} e título: {Titulo}. Erros: {Erros}", idTask, editDto.Title, string.Join(", ", validatorResult.Errors.Select(e => e.ErrorMessage)));

                var result = Result.Fail<GetTaskDto>("Erro de validacao");

                var errors = validatorResult.Errors
                    .Select(e => new Error(e.ErrorMessage))
                    .ToList();

                result.WithErrors(errors);

                return result;
            }

            _logger.LogInformation("Validação concluída com sucesso para tarefa com ID: {Id}", idTask);

            var taskE = _mapper.Map<TaskE>(editDto);
            taskE.Id = idTask;
            taskE.UsuarioId = userId;

            _logger.LogInformation("Entidade mapeada para tarefa com ID: {Id}. Iniciando atualização...", idTask);

            await _repository.EditTask(taskE);

            _logger.LogInformation("Tarefa com ID: {Id} atualizada com sucesso no repositório", idTask);

            var taskGet = _mapper.Map<GetTaskDto>(taskE);

            return Result.Ok(taskGet);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao editar a tarefa com ID: {Id}", idTask);
            return Result.Fail<GetTaskDto>($"Erro interno inesperado: {ex.Message}");
        }
    }
}