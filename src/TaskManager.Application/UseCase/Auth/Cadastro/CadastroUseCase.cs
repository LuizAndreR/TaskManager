using AutoMapper;
using FluentResults;
using FluentValidation;
using Microsoft.Extensions.Logging;
using TaskManager.Application.Dtos.Auth;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Interfaces.Auth;
using TaskManager.Infrastructure.Auth.Generator;

namespace TaskManager.Application.UseCase.Auth.Cadastro;

public class CadastroUseCase : ICadastroUseCase
{
    private readonly IValidator<CadastroUsuarioRequest> _validator;
    private readonly IUsuarioRepository _repository;
    private readonly IMapper _mapper;
    private readonly IJwtGenerator _tokenService;
    private readonly ILogger<CadastroUseCase> _logger;

    public CadastroUseCase(IValidator<CadastroUsuarioRequest> validator, IUsuarioRepository repository, IMapper mapper, IJwtGenerator tokenService, ILogger<CadastroUseCase> logger)
    {
        _validator = validator;
        _repository = repository;
        _mapper = mapper;
        _tokenService = tokenService;
        _logger = logger;
    }

    public async Task<Result<string>> Execute(CadastroUsuarioRequest request)
    {
        try
        {
            _logger.LogInformation("Iniciando processo de cadastro para o e-mail: {Email}", request.Email);

            var validationResult = _validator.Validate(request);
            if (!validationResult.IsValid)
            {
                var result = Result.Fail("Erro de validação");
                foreach (var erro in validationResult.Errors)
                {
                    result.WithError(erro.ErrorMessage);
                }

                _logger.LogWarning("Validação falhou para o e-mail {Email}. Erros: {@Erros}", request.Email, validationResult.Errors);
                return result;
            }

            if (await _repository.EmailExiste(request.Email))
            {
                _logger.LogWarning("Tentativa de cadastro com e-mail já existente: {Email}", request.Email);
                return Result.Fail("Email já cadastrado");
            }

            var senhaHash = BCrypt.Net.BCrypt.HashPassword(request.Senha, 12);

            var usuario = _mapper.Map<Usuario>(request);
            usuario.SenhaHash = senhaHash;

            usuario = await _repository.Salve(usuario);

            _logger.LogInformation("Cadastro concluído com sucesso para o e-mail: {Email}", request.Email);

            var token = _tokenService.Generate(usuario.Email, usuario.Id);

            _logger.LogInformation("Token gerado com sucesso para o e-mail {Email}", usuario.Email);
            return Result.Ok(token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao cadastrar usuário com e-mail: {Email}", request.Email);
            return Result.Fail($"Erro interno inesperado: {ex.Message}");
        }
    }

}

