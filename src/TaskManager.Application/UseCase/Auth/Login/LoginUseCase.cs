using FluentResults;
using FluentValidation;
using Microsoft.Extensions.Logging;
using TaskManager.Application.Dtos.Auth;
using TaskManager.Domain.Interfaces.Auth;
using TaskManager.Infrastructure.Auth.Generator;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;

namespace TaskManager.Application.UseCase.Auth.Login;
public class LoginUseCase : ILoginUseCase
{
    private readonly IValidator<LoginUsuarioRequest> _validator;
    private readonly IUsuarioRepository _repository;
    private readonly IJwtGenerator _tokenService;
    private readonly ILogger<LoginUseCase> _logger;

    public LoginUseCase(IValidator<LoginUsuarioRequest> validator, IUsuarioRepository repository, IJwtGenerator tokenService, ILogger<LoginUseCase> logger)
    {
        _validator = validator;
        _repository = repository;
        _tokenService = tokenService;
        _logger = logger;
    }

    public async Task<Result<string>> Execute(LoginUsuarioRequest request)
    {
        try
        {
            _logger.LogInformation("Iniciando processo de login para {Email}", request.Email);

            var validator = _validator.Validate(request);

            if (!validator.IsValid)
            {
                var result = Result.Fail("Erro de validação");

                foreach (var erro in validator.Errors)
                {
                    result.WithError(erro.ErrorMessage);
                }

                _logger.LogWarning("Erro de validação ao tentar logar {Email}: {@Erros}", request.Email, validator.Errors);
                return result;
            }

            var usuario = await _repository.BuscaUsuario(request.Email);

            if (usuario == null)
            {
                _logger.LogWarning("Tentativa de login com email inexistente: {Email}", request.Email);
                return Result.Fail("Usuário ou senha inválidos.");
            }

            if (!BCrypt.Net.BCrypt.Verify(request.Senha, usuario.SenhaHash))
            {
                _logger.LogWarning("Senha incorreta para o email: {Email}", request.Email);
                return Result.Fail("Usuário ou senha inválidos.");
            }

            var token = _tokenService.Generate(request.Email);

            _logger.LogInformation("Login realizado com sucesso para {Email}", request.Email);
            return Result.Ok(token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao realizar login para {Email}", request.Email);
            return Result.Fail("Erro interno inesperado: " + ex.Message);
        }
    }
}
