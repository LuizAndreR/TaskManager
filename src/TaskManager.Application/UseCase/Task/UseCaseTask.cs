using AutoMapper;
using FluentResults;
using FluentValidation;
using Microsoft.Extensions.Logging;
using TaskManager.Application.Dtos.TaskDto;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Interfaces.ITask;

namespace TaskManager.Application.UseCase.Task;

public class UseCaseTask : IUseCaseTask
{
    private readonly ITaskRepository _repository;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateTaskDto> _validator;
    
    private readonly ILogger<UseCaseTask> _logger;

    public UseCaseTask(ITaskRepository repository, IMapper mapper, IValidator<CreateTaskDto> validator, ILogger<UseCaseTask> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
        _validator = validator;
    }

    public async Task<Result<IEnumerable<GetTaskDto>>> BuscaTasksbyIdUserAsync(int userId)
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

            _logger.LogInformation("{Quantidade} tarefa(s) encontrada(s) para o usuário com ID {UserId}", tarefasDto.Count(), userId);

            return Result.Ok(tarefasDto);

        }catch(Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao busca tarefas usuário: {Id}", userId);
            return Result.Fail($"Erro interno inesperado: {ex.Message}");
        }
    }

    public async Task<Result<GetTaskDto>> BuscaTaskbyIdAsync(int id, int userId)
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

    public async Task<Result<GetTaskDto>> EditTaskAsync(CreateTaskDto editDto, int idTask, int userId)
    {
        try
        {
            _logger.LogInformation("Iniciando validação para edição da tarefa com ID: {Id} e título: {Titulo}", idTask, editDto.Title);

            var validatorResult = _validator.Validate(editDto);

            if (!validatorResult.IsValid)
            {
                _logger.LogWarning("Validação falhou ao editar tarefa com ID: {Id} e título: {Titulo}. Erros: {Erros}", 
                    idTask, 
                    editDto.Title, 
                    string.Join(", ", validatorResult.Errors.Select(e => e.ErrorMessage)));

                var result = Result.Fail<GetTaskDto>("Erro de validação");

                foreach (var erro in validatorResult.Errors)
                {
                    result.WithError(erro.ErrorMessage);
                }

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
