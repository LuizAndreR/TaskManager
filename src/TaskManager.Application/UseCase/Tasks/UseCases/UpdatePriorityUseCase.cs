using AutoMapper;
using FluentResults;
using FluentValidation;
using Microsoft.Extensions.Logging;
using TaskManager.Application.Dtos.TaskDto;
using TaskManager.Application.UseCase.Tasks.Interfaces;
using TaskManager.Domain.Interfaces.ITask;

namespace TaskManager.Application.UseCase.Tasks.UseCases;

public class UpdatePriorityUseCase : IUpdatePriorityUseCase
{
    private readonly ITaskRepository _repository;
    private readonly IMapper _mapper;
    private readonly IValidator<UpdatePriorityDto> _validator;
    private readonly ILogger<UpdatePriorityUseCase> _logger;

    public UpdatePriorityUseCase(ITaskRepository repository, IMapper mapper,
        IValidator<UpdatePriorityDto> validator, ILogger<UpdatePriorityUseCase> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result> UpdatePriorityAsync(UpdatePriorityDto request, int taskId, int userId)
    {
        try
        {
            _logger.LogInformation("Iniciando validação para edição de prioridade da tarefa com ID: {Id} para prioridade {Priority}", taskId, request.Priority);

            var validatorResult = _validator.Validate(request);

            if (!validatorResult.IsValid)
            {
                _logger.LogWarning("Validação falhou ao atualizar prioridade da tarefa com id {Id}. Erros: {Errors}", taskId, validatorResult.Errors);

                var result = Result.Fail("Erro de validacao");

                var errors = validatorResult.Errors
                    .Select(e => new Error(e.ErrorMessage))
                    .ToList();

                result.WithErrors(errors);

                return result;
            }

            _logger.LogInformation("Validação concluída com sucesso para tarefa com ID: {Id}", taskId);

            var task = await _repository.GetId(taskId, userId);

            task!.Priority = request.Priority;

            await _repository.EditTask(task);

            _logger.LogInformation("Prioridade da tarefa com ID: {Id} atualizada com sucesso para {Priority}", taskId, request.Priority);

            return Result.Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar prioridade da tarefa com ID: {Id}", taskId);
            return Result.Fail("Erro ao atualizar prioridade da tarefa");
        }
    }
}
