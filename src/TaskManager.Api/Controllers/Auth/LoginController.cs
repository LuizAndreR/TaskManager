using FluentResults;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.Dtos.Auth;
using TaskManager.Application.UseCase.Auth.Login;

namespace TaskManager.Api.Controllers.Auth.Login;

[Controller]
[Route("/auth/login")]
public class LoginController : ControllerBase
{
    private readonly ILoginUseCase _useCase;
    private readonly ILogger<LoginController> _logger;

    public LoginController(ILoginUseCase useCase, ILogger<LoginController> logger)
    {
        _useCase = useCase;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> Login([FromBody]LoginUsuarioRequest request)
    {
        _logger.LogInformation("Recebida requisição de login para {Email}", request.Email);

        Result<string> result = await _useCase.Execute(request);
        return MapResultHttpResponse(result, request.Email);
    }

    private IActionResult MapResultHttpResponse(Result<string> result, string email)
    {
        if (result.IsSuccess)
        {
            _logger.LogInformation("Retornando 200 OK para o login do usuário {Email}", email);
            return Ok(new { token = result.Value });
        }

        var erro = result.Errors.First().Message;

        if (erro == "Erro de validação")
        {
            _logger.LogWarning("Retornando 400 BadRequest para o login do usuário {Email}", email);
            return BadRequest(result.Errors.Select(e => e.Message));
        }

        if (erro == "Usuário ou senha inválidos.")
        {
            _logger.LogWarning("Retornando 401 Unauthorized para o login do usuário {Email}", email);
            return Unauthorized(result.Errors.First().Message);
        }

        _logger.LogError("Retornando 500 InternalServerError para o login do usuário {Email}. Erro: {Erro}", email, erro);
        return StatusCode(500, erro);
    }

}
