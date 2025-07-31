using FluentResults;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Api.Extensions;
using TaskManager.Application.Dtos.TaskDto;
using TaskManager.Application.UseCase.Task;

namespace TaskManager.Api.Controllers.Task;

[ApiController]
[Route("/task/")]
[Authorize]
public class TaskController : ControllerBase
{
    private readonly IUseCaseTask _userCase;
    private readonly ILogger<TaskController> _logger;

    public TaskController(IUseCaseTask userCase, ILogger<TaskController> logger)
    {
        _userCase = userCase;
        _logger = logger;
    }

    [HttpGet("getall")]
    public async Task<IActionResult> GetAll()
    {
        _logger.LogInformation("Recebido requiseção de busca de tarefas de um usuario");

        var userId = User.GetUserId();

        if (userId.IsFailed)
        {
            _logger.LogInformation("Retornando 401 Unauthorized para requisição sem token ou não válido");
            return Unauthorized();
        }

        var result = await _userCase.BuscaTasksbyIdUserAsync(userId.Value);

        if (result.IsFailed)
        {
            var firstError = result.Errors.First();

            if (firstError.Message.Contains("Nenhuma tarefa encontrada"))
            {
                _logger.LogInformation("Nenhuma tarefa encontrada para o usuário {UserId}", userId.Value);
                return NotFound(new { message = firstError.Message });
            }

            _logger.LogError("Erro inesperado ao buscar tarefas para o usuário {UserId}. Erro: {Erro}", userId.Value, firstError.Message);
            return StatusCode(500, new { message = "Erro interno inesperado ao buscar tarefas." });
        }

        _logger.LogInformation("Retornando 200 OK para usuário {UserId} com {Count} tarefas", userId.Value, result.Value.Count());
        return Ok(result.Value);
    }

    [HttpGet("getid/{id}")]
    public async Task<IActionResult> GetId(int id)
    {
        _logger.LogInformation("Recebido requiseção de busca de tarefa pelo id");

        var userId = User.GetUserId();

        if (userId.IsFailed)
        {
            _logger.LogInformation("Retornando 401 Unauthorized para requisição sem token ou não válido");
            return Unauthorized();
        }

        var result = await _userCase.BuscaTaskbyIdAsync(id, userId.Value);

        if (result.IsFailed)
        {
            var firstError = result.Errors.First();

            if (firstError.Message.Contains("Nenhuma tarefa encontrada"))
            {
                _logger.LogInformation("Nenhuma tarefa encontrada com id {Id}", id);
                return NotFound(new { message = firstError.Message });
            }

            _logger.LogError("Erro inesperado ao buscar tarefas com id {Id} Erro: {Erro}", id, firstError.Message);
            return StatusCode(500, new { message = "Erro interno inesperado ao buscar tarefas." });
        }

        _logger.LogInformation("Retornando 200 OK para usuário {UserId} com a tarefa. Id: {Id}", userId.Value, id);
        return Ok(result.Value);
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateTaskDto request)
    {
        _logger.LogInformation("Requisição recebida para cadastra nova tarefa: {Nome}", request.Title);

        var userId = User.GetUserId();

        if (userId.IsFailed)
        {
            _logger.LogInformation("Retornando 401 Unauthorized para requisição sem token ou não válido");
            return Unauthorized();
        }

        Result<GetTaskDto> result = await _userCase.CreateTaskAsync(request, userId.Value);

        if (result.IsSuccess)
        {
            _logger.LogInformation("Tarefa criada com susseso: {Nome}~. Código HTTP: 201", request.Title);
            return CreatedAtAction(nameof(GetId), new { id = result.Value.Id }, result.Value);
        }

        var erro = result.Errors.First().Message;

        if (erro.Contains("validação"))
        {
            _logger.LogWarning("Falha de validação ao cadastrar nova tarefa {Tarefas^}. Erros: {@Erros}. Código HTTP: 400", request.Title, result.Errors);
            return BadRequest(result.Errors.Select(e => e.Message));
        }

        _logger.LogError("Erro inesperado ao cadastrar {Nome}. Mensagem: {Erro}. Código HTTP: 500",request.Title, erro);
        return StatusCode(500, erro);
    }
}
