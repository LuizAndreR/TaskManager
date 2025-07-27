using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using TaskManager.Application.Dtos.Auth;
using TaskManager.Application.UseCase.Auth.Cadastro;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Interfaces.Auth;
using TaskManager.Infrastructure.Auth.Generator;
using TaskManager.Tests.Utils;

namespace TaskManager.Tests.UseCase;
public class CadastroUseCaseTests
{
    private CadastroUseCase _useCase;
    private Mock<IValidator<CadastroUsuarioRequest>> _validator;
    private Mock<IUsuarioRepository> _repository;
    private IMapper _mapper;
    private Mock<IJwtGenerator> _jwtGenerator;
    private Mock<ILogger<CadastroUseCase>> _logger;


    public CadastroUseCaseTests()
    {
        _validator = new Mock<IValidator<CadastroUsuarioRequest>>();
        _repository = new Mock<IUsuarioRepository>();
        _jwtGenerator = new Mock<IJwtGenerator>();
        _logger = new Mock<ILogger<CadastroUseCase>>();

        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<AutoMapperTestProfile>();
        });
        _mapper = config.CreateMapper();

        _useCase = new CadastroUseCase(
              _validator.Object,
              _repository.Object,
              _mapper,
              _jwtGenerator.Object,
              _logger.Object
        );
    }

    [Fact]
    public async Task Deve_Cadastra_Usuario_Susseso()
    {
        var request = new CadastroUsuarioRequest
        {
            Nome = "Teste",
            Email = "teste@gmail.com",
            Senha = "Teste2@"
        };

        _validator
            .Setup(v => v.Validate(request))
            .Returns(new ValidationResult());

        _repository
            .Setup(r => r.EmailExiste(request.Email))
            .ReturnsAsync(false);

        _repository
            .Setup(r => r.Salve(It.IsAny<Usuario>()))
            .Returns(Task.CompletedTask);

        var resultado = await _useCase.Execute(request);

        Assert.True(resultado.IsSuccess);
        _repository.Verify(r => r.Salve(It.IsAny<Usuario>()), Times.Once); 
    }
    [Fact]
    public async Task Deve_Falhar_Quando_Email_Ja_Estiver_Cadastrado()
    {
        var request = new CadastroUsuarioRequest
        {
            Nome = "Luiz",
            Email = "luiz@email.com",
            Senha = "Senha123!",
        };

        _validator
            .Setup(v => v.Validate(request))
            .Returns(new ValidationResult());

        _repository
            .Setup(r => r.EmailExiste(request.Email))
            .ReturnsAsync(true);

        var resultado = await _useCase.Execute(request);

        Assert.False(resultado.IsSuccess);
        Assert.Contains(resultado.Errors, e => e.Message.Contains("Email já cadastrado"));
        _repository.Verify(r => r.Salve(It.IsAny<Usuario>()), Times.Never);
    }

    [Fact]
    public async Task Deve_Retornar_Erro_Quando_Excecao_Inesperada_Ocorre()
    {
        var request = new CadastroUsuarioRequest
        {
            Nome = "Luiz",
            Email = "luiz@email.com",
            Senha = "Senha123!",
        };

        _validator
            .Setup(v => v.Validate(request))
            .Returns(new ValidationResult());

        _repository
            .Setup(r => r.EmailExiste(It.IsAny<string>()))
            .ThrowsAsync(new Exception("Falha no banco"));

        var resultado = await _useCase.Execute(request);

        Assert.False(resultado.IsSuccess);
        Assert.Contains("Erro interno inesperado", resultado.Errors[0].Message);
    }

    [Fact]
    public async Task Deve_Falhar_Quando_Sem_Nome_E_Sem_Email()
    {
        var request = new CadastroUsuarioRequest
        {
            Nome = "",
            Email = "luiz.com",
            Senha = "Senha123!",
        };

        _validator
            .Setup(v => v.Validate(request))
            .Returns(new ValidationResult(new List<ValidationFailure>
            {
                new ValidationFailure("Nome", "O nome é obrigatório"),
                new ValidationFailure("Email", "E-mail inválido")
            }));

        _repository
            .Setup(r => r.EmailExiste(request.Email))
            .ReturnsAsync(false);

        var resultado = await _useCase.Execute(request);

        Assert.False(resultado.IsSuccess);
        Assert.Contains(resultado.Errors, e => e.Message.Contains("validação"));
        _repository.Verify(r => r.Salve(It.IsAny<Usuario>()), Times.Never);
    }
}
