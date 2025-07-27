using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using TaskManager.Application.Dtos.Auth;
using TaskManager.Application.UseCase.Auth.Login;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Interfaces.Auth;
using TaskManager.Infrastructure.Auth.Generator;

namespace TaskManager.Tests.UseCase;

public class LoginUseCaseTest
{
    private LoginUseCase _useCase;
    private Mock<IValidator<LoginUsuarioRequest>> _validator;
    private Mock<IUsuarioRepository> _repository;
    private Mock<IJwtGenerator> _jwtGenerator;
    private Mock<ILogger<LoginUseCase>> _logger;

    public LoginUseCaseTest()
    {
        _validator = new Mock<IValidator<LoginUsuarioRequest>>();
        _repository = new Mock<IUsuarioRepository>();
        _jwtGenerator = new Mock<IJwtGenerator>();
        _logger = new Mock<ILogger<LoginUseCase>>();

        _useCase = new LoginUseCase(
              _validator.Object,
              _repository.Object,
              _jwtGenerator.Object,
              _logger.Object
        );
    }

    [Fact]
    public async Task Execute_DeveRetornarFalha_QuandoValidacaoFalha()
    {
        var request = new LoginUsuarioRequest
        {
            Email = "email@teste.com",
            Senha = "senha"
        };

        _validator.Setup(v => v.Validate(request))
            .Returns(new ValidationResult(new List<ValidationFailure>
            {
                new ValidationFailure("Email", "Email inválido")
            }));

        var result = await _useCase.Execute(request);

        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, e => e.Message == "Erro de validação");
        Assert.Contains(result.Errors, e => e.Message == "Email inválido");
    }

    [Fact]
    public async Task Execute_DeveRetornarFalha_QuandoUsuarioNaoEncontrado()
    {
        var request = new LoginUsuarioRequest 
        { 
            Email = "email@teste.com", 
            Senha = "senha" 
        };

        _validator.Setup(v => v.Validate(request))
            .Returns(new ValidationResult());

        _repository.Setup(r => r.BuscaUsuario(request.Email))
            .ReturnsAsync((Usuario?)null);

        var result = await _useCase.Execute(request);

        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, e => e.Message == "Usuário ou senha inválidos.");
    }

    [Fact]
    public async Task Execute_DeveRetornarFalha_QuandoSenhaIncorreta()
    {
        var request = new LoginUsuarioRequest { Email = "email@teste.com", Senha = "senhaErrada" };

        _validator.Setup(v => v.Validate(request))
            .Returns(new ValidationResult());

        var usuario = new Usuario
        {
            Nome = "teste",
            Email = request.Email,
            SenhaHash = BCrypt.Net.BCrypt.HashPassword("senhaCorreta")
        };

        _repository.Setup(r => r.BuscaUsuario(request.Email))
            .ReturnsAsync(usuario);

        var result = await _useCase.Execute(request);

        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, e => e.Message == "Usuário ou senha inválidos.");
    }

    [Fact]
    public async Task Execute_DeveRetornarSucesso_QuandoLoginValido()
    {
        var request = new LoginUsuarioRequest { Email = "email@teste.com", Senha = "senhaCorreta" };
        var tokenEsperado = "token123";

        _validator.Setup(v => v.Validate(request))
            .Returns(new ValidationResult()); 

        var usuario = new Usuario
        {
            Nome = "teste",
            Email = request.Email,
            SenhaHash = BCrypt.Net.BCrypt.HashPassword(request.Senha)
        };

        _repository.Setup(r => r.BuscaUsuario(request.Email))
            .ReturnsAsync(usuario);

        _jwtGenerator.Setup(t => t.Generate(request.Email))
            .Returns(tokenEsperado);

        var result = await _useCase.Execute(request);

        Assert.True(result.IsSuccess);
        Assert.Equal(tokenEsperado, result.Value);
    }

    [Fact]
    public async Task Deve_Retornar_Erro_Quando_Excecao_Inesperada_Ocorre()
    {
        var request = new LoginUsuarioRequest
        {
            Email = "luiz@email.com",
            Senha = "Senha123!"
        };

        _validator
            .Setup(v => v.Validate(request))
            .Returns(new ValidationResult());

        _repository
            .Setup(r => r.BuscaUsuario(It.IsAny<string>()))
            .ThrowsAsync(new Exception("Falha no banco"));

        var resultado = await _useCase.Execute(request);

        Assert.False(resultado.IsSuccess);
        Assert.Contains(resultado.Errors, e => e.Message.Contains("Erro interno inesperado"));
    }

}
