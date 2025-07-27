using FluentValidation;
using TaskManager.Application.Dtos.Auth;
using TaskManager.Application.Validator.Auth;

namespace TaskManager.Tests.Validator;

public class LoginValidatorTest
{
    private readonly LoginValidator _validator = new();

    [Fact]
    public void Deve_Retornar_Susseso_Sem_Telefone()
    {
        var request = new LoginUsuarioRequest
        {
            Email = "email@valido.com",
            Senha = "Luiz2004!"
        };

        var result = _validator.Validate(request);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Deve_Retornar_Erro_Quando_Email_Invalido()
    {
        var request = new LoginUsuarioRequest
        {
            Email = "teste.com",
            Senha = "Senha123!"
        };

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Email");
    }

    [Fact]
    public void Deve_Falhar_Quando_Senha_Invalida()
    {
        var request = new LoginUsuarioRequest
        {
            Email = "email@valido.com",
            Senha = ""
        };

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Senha");
    }
}
