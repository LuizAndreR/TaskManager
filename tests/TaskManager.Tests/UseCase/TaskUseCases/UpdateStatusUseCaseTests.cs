using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using FluentResults;
using TaskManager.Application.Dtos.TaskDto;
using TaskManager.Application.UseCase.Tasks.UseCases;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Interfaces.ITask;

namespace TaskManager.Tests.UseCase.TaskUseCases;

public class UpdateStatusUseCaseTests
{
    private readonly Mock<ITaskRepository> _repoMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IValidator<UpdateStatusTaskDto>> _validatorMock;
    private readonly UpdateStatusTaskUseCase _useCase;

    public UpdateStatusUseCaseTests()
    {
        _repoMock = new Mock<ITaskRepository>();
        _mapperMock = new Mock<IMapper>();
        _validatorMock = new Mock<IValidator<UpdateStatusTaskDto>>();
        var loggerMock = new Mock<ILogger<UpdateStatusTaskUseCase>>();

        _useCase = new UpdateStatusTaskUseCase(_repoMock.Object, _mapperMock.Object, _validatorMock.Object,loggerMock.Object);
    }

    [Fact]
    public async Task UpdateStatusAsync_DeveRetornarOk_QuandoDadosValidos()
    {
        var dto = new UpdateStatusTaskDto { Status = "Concluída" };
        var usuario = new Usuario { Id = 5, Nome = "Teste", Email = "teste@gmail.com", SenhaHash = "Teste" };

        var taskEntity = new TaskE
        {
            Id = 1,
            Status = "Pendente",
            UsuarioId = usuario.Id,
            Usuario = usuario
        };

        _validatorMock.Setup(v => v.Validate(dto)).Returns(new ValidationResult());
        _repoMock.Setup(r => r.GetId(taskEntity.Id, usuario.Id)).ReturnsAsync(taskEntity);
        _repoMock.Setup(r => r.EditTask(taskEntity)).Returns(Task.CompletedTask);

        var result = await _useCase.UpdateStatusAsync(dto, taskEntity.Id, usuario.Id);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task UpdateStatusAsync_DeveRetornarErro_QuandoValidacaoFalhar()
    {
        var dto = new UpdateStatusTaskDto { Status = "" };
        var validationResult = new ValidationResult(new List<ValidationFailure>
        {
            new ValidationFailure("Status", "Status é obrigatório")
        });

        _validatorMock.Setup(v => v.Validate(dto)).Returns(validationResult);

        var result = await _useCase.UpdateStatusAsync(dto, 1, 5);

        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, e => e.Message == "Status é obrigatório");
    }

    [Fact]
    public async Task UpdateStatusAsync_DeveRetornarErro_QuandoExcecaoForLancada()
    {
        var dto = new UpdateStatusTaskDto { Status = "Concluída" };
        var usuario = new Usuario { Id = 5, Nome = "Teste", Email = "teste@gmail.com", SenhaHash = "Teste" };

        var taskEntity = new TaskE
        {
            Id = 1,
            Status = "Pendente",
            UsuarioId = usuario.Id,
            Usuario = usuario
        };

        _validatorMock.Setup(v => v.Validate(dto)).Returns(new ValidationResult());
        _repoMock.Setup(r => r.GetId(taskEntity.Id, usuario.Id)).ThrowsAsync(new Exception("Falha no banco"));

        var result = await _useCase.UpdateStatusAsync(dto, taskEntity.Id, usuario.Id);

        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, e => e.Message.Contains("Falha no banco"));
    }
}
