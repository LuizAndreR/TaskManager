using FluentResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TaskManager.Api.Controllers.Auth;
using TaskManager.Application.Dtos.Auth;
using TaskManager.Application.UseCase.Auth.Cadastro;

namespace TaskManager.Tests.Controllers;

public class CadastroControllerTests
{
    private Mock<ICadastroUseCase> _useCase;
    private CadastroController _controller;
    private Mock<ILogger<CadastroController>> _logger;

    public CadastroControllerTests()
    {
        _useCase = new Mock<ICadastroUseCase>();
        _logger = new Mock<ILogger<CadastroController>>();
        _controller = new CadastroController(_useCase.Object, _logger.Object);
    }

    [Fact]
    public async Task Cadastrar_DeveRetornar201_QuandoUsuarioValido()
    {
        var request = new CadastroUsuarioRequest
        {
            Nome = "Luiz",
            Email = "luiz@email.com",
            Senha = "Senha123!"
        };

        _useCase.Setup(x => x.Execute(It.IsAny<CadastroUsuarioRequest>()))
            .ReturnsAsync(Result.Ok());

        var resultado = await _controller.CadastroDeUsuario(request);

        var objectResult = Assert.IsType<CreatedResult>(resultado);
        Assert.Equal(201, objectResult.StatusCode);
    }

    [Fact]
    public async Task Cadastrar_DeveRetornar409_QuandoEmailJaExiste()
    {
        var request = new CadastroUsuarioRequest
        {
            Nome = "Luiz",
            Email = "luiz@gmail.com",
            Senha = "Senha123!"
        };

        _useCase.Setup(x => x.Execute(It.IsAny<CadastroUsuarioRequest>()))
            .ReturnsAsync(Result.Fail("Email já cadastrado"));

        var resultado = await _controller.CadastroDeUsuario(request);

        var objectResult = Assert.IsType<ConflictResult>(resultado);
        Assert.Equal(409, objectResult.StatusCode);
    }

    [Fact]
    public async Task Cadastrar_DeveRetornar400_QuandoDadosInvalidos()
    {
        var request = new CadastroUsuarioRequest
        {
            Nome = "Luiz",
            Email = "luizemail.com",
            Senha = "Luiz2004!"
        };

        _useCase.Setup(x => x.Execute(It.IsAny<CadastroUsuarioRequest>()))
            .ReturnsAsync(Result.Fail("Erro de validação"));

        var resultado = await _controller.CadastroDeUsuario(request);

        var objectResult = Assert.IsType<BadRequestObjectResult>(resultado);
        Assert.Equal(400, objectResult.StatusCode);

        var erros = Assert.IsAssignableFrom<IEnumerable<string>>(objectResult.Value);
        Assert.Contains("Erro de validação", erros);
    }

    [Fact]
    public async Task Cadastrar_DeveRetornar500_QuandoErroInesperado()
    {
        var request = new CadastroUsuarioRequest
        {
            Nome = "Luiz",
            Email = "luizemail.com",
            Senha = "2313d"
        };

        _useCase.Setup(x => x.Execute(It.IsAny<CadastroUsuarioRequest>()))
            .ReturnsAsync(Result.Fail("Erro interno inesperado:"));

        var resultado = await _controller.CadastroDeUsuario(request);

        var objectResult = Assert.IsType<ObjectResult>(resultado);
        Assert.Equal(500, objectResult.StatusCode);
        Assert.Equal("Erro interno inesperado:", objectResult.Value);
    }
}
