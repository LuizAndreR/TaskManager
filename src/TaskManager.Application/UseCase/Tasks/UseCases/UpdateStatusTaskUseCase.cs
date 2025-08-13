using AutoMapper;
using FluentResults;
using FluentValidation;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using TaskManager.Application.Dtos.TaskDto;
using TaskManager.Application.UseCase.Tasks.Interfaces;
using TaskManager.Domain.Interfaces.ITask;

namespace TaskManager.Application.UseCase.Tasks.UseCases;

public class UpdateStatusTaskUseCase : IUpdateStatusTaskUseCase
{
    private readonly ITaskRepository _repository;
    private readonly IMapper _mapper;
    private readonly IValidator<UpdateStatusTaskDto> _validator;
    private readonly ILogger<UpdateStatusTaskUseCase> _logger;

    public UpdateStatusTaskUseCase(ITaskRepository repository, IMapper mapper,
        IValidator<UpdateStatusTaskDto> validator, ILogger<UpdateStatusTaskUseCase> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result> UpdateStatusAsync(UpdateStatusTaskDto request, int taskId, int userId)
    {
        try
        {
            _logger.LogInformation("Iniciando validação para edição de status da tarefa com ID: {Id} para status {Status}", taskId, request.Status);

            var validatorResult = _validator.Validate(request);

            if (!validatorResult.IsValid)
            {
                _logger.LogWarning("Validação falhou ao atualizar status da tarefa com id {Id}. Erros: {Errors}", taskId, validatorResult.Errors);

                var result = Result.Fail("Erro de validacao");

                var errors = validatorResult.Errors
                    .Select(e => new Error(e.ErrorMessage))
                    .ToList();

                result.WithErrors(errors);

                return result;
            }

            _logger.LogInformation("Validação concluída com sucesso para tarefa com ID: {Id}", taskId);

            var task = await _repository.GetId(taskId, userId);

            task!.Status = request.Status;

            await _repository.EditTask(task);

            _logger.LogInformation("Status da tarefa com ID: {Id} atualizado com sucesso para {Status}", taskId, request.Status);

            return Result.Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao editar o status da tarefa com ID: {Id}", taskId);
            return Result.Fail($"Erro interno inesperado: {ex.Message}");
        }
    }
}