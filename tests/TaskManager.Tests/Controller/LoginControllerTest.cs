using FluentResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TaskManager.Api.Controllers.Auth.Login;
using TaskManager.Application.Dtos.Auth;
using TaskManager.Application.UseCase.Auth.Login;

namespace TaskManager.Tests.Controller;

public class LoginControllerTest
{
    private Mock<ILoginUseCase> _useCase;
    private LoginController _controller;
    private Mock<ILogger<LoginController>> _logger;

    public LoginControllerTest()
    {
        _useCase = new Mock<ILoginUseCase>();
        _logger = new Mock<ILogger<LoginController>>();

        _controller = new LoginController(_useCase.Object, _logger.Object);
    }

    [Fact]
    public async Task Login_DeveRetornar200_QuandoUsuarioValido()
    {
        var request = new LoginUsuarioRequest
        {
            Email = "luiz@email.com",
            Senha = "Senha123!"
        };

        _useCase.Setup(x => x.Execute(It.IsAny<LoginUsuarioRequest>()))
            .ReturnsAsync(Result.Ok());

        var resultado = await _controller.Login(request);

        var objectResult = Assert.IsType<OkObjectResult>(resultado);
        Assert.Equal(200, objectResult.StatusCode);
    }

    [Fact]
    public async Task Login_DeveRetornar400_QuandoUsuarioInvalido()
    {
        var request = new LoginUsuarioRequest
        {
            Email = "luizemail.com",
            Senha = ""
        };

        _useCase.Setup(x => x.Execute(It.IsAny<LoginUsuarioRequest>()))
            .ReturnsAsync(Result.Fail("Erro de validação"));

        var resultado = await _controller.Login(request);

        var objectResult = Assert.IsType<BadRequestObjectResult>(resultado);
        Assert.Equal(400, objectResult.StatusCode);

        var erros = Assert.IsAssignableFrom<IEnumerable<string>>(objectResult.Value);
        Assert.Contains("Erro de validação", erros);
    }

    [Fact]
    public async Task Login_DeveRetornar401_QuandoSenhaInvalida()
    {
        var request = new LoginUsuarioRequest
        {
            Email = "luize@gmail.com",
            Senha = "wdadwa"
        };

        _useCase.Setup(x => x.Execute(It.IsAny<LoginUsuarioRequest>()))
            .ReturnsAsync(Result.Fail("Usuário ou senha inválidos."));

        var resultado = await _controller.Login(request);

        var objectResult = Assert.IsType<UnauthorizedObjectResult>(resultado);
        Assert.Equal(401, objectResult.StatusCode);
        Assert.Equal("Usuário ou senha inválidos.", objectResult.Value);
    }

    [Fact]
    public async Task Login_DeveRetornar500_QuandoErroInesperado()
    {
        var request = new LoginUsuarioRequest
        {
            Email = "luizemail.com",
            Senha = "2313d"
        };

        _useCase.Setup(x => x.Execute(It.IsAny<LoginUsuarioRequest>()))
            .ReturnsAsync(Result.Fail("Erro interno inesperado:"));

        var resultado = await _controller.Login(request);

        var objectResult = Assert.IsType<ObjectResult>(resultado);
        Assert.Equal(500, objectResult.StatusCode);
        Assert.Equal("Erro interno inesperado:", objectResult.Value);
    }
}
