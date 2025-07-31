using FluentResults;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.Dtos.Auth;
using TaskManager.Application.UseCase.Auth.Cadastro;

namespace TaskManager.Api.Controllers.Auth;

[ApiController]
[Route("/auth/register")]
public class CadastroController : ControllerBase
{
    private readonly ICadastroUseCase _useCase;
    private readonly ILogger<CadastroController> _logger;

    public CadastroController(ICadastroUseCase useCase, ILogger<CadastroController> logger)
    {
        _useCase = useCase;
        _logger = logger;
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> CadastroDeUsuario([FromBody] CadastroUsuarioRequest request)
    {
        _logger.LogInformation("Requisição recebida para cadastro do e-mail {Email}", request.Email);

        Result<string> result = await _useCase.Execute(request);
        return MapResultHttpResponse(result, request.Email);
    }

    private IActionResult MapResultHttpResponse(Result<string> result, string email)
    {
        if (result.IsSuccess)
        {
            _logger.LogInformation("Token gerado com sucesso para o e-mail {Email}. Código HTTP: 201", email);
            return Created(string.Empty, new { token = result.Value });
        }

        var erro = result.Errors.First().Message;

        if (erro.Contains("Email já cadastrado"))
        {
            _logger.LogWarning("Conflito: e-mail já cadastrado - {Email}. Código HTTP: 409", email);
            return Conflict();
        }

        if (erro.Contains("validação"))
        {
            _logger.LogWarning("Falha de validação ao cadastrar {Email}. Erros: {@Erros}. Código HTTP: 400", email, result.Errors);
            return BadRequest(result.Errors.Select(e => e.Message));
        }

        _logger.LogError("Erro inesperado ao cadastrar {Email}. Mensagem: {Erro}. Código HTTP: 500", email, erro);
        return StatusCode(500, erro);
    }


}
