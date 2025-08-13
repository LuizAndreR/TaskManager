using System.ComponentModel.DataAnnotations;
using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using TaskManager.Application.Dtos.TaskDto;
using TaskManager.Application.UseCase.Tasks.UseCases;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Interfaces.ITask;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace TaskManager.Tests.UseCase.TaskUseCases;

public class UpdatePriorityUseCaseTests
{
    private readonly Mock<ITaskRepository> _repoMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IValidator<UpdatePriorityDto>> _validatorMock;
    private readonly UpdatePriorityUseCase _useCase;

    public UpdatePriorityUseCaseTests()
    {
        _repoMock = new Mock<ITaskRepository>();
        _mapperMock = new Mock<IMapper>();
        _validatorMock = new Mock<IValidator<UpdatePriorityDto>>();
        var loggerMock = new Mock<ILogger<UpdatePriorityUseCase>>();

        _useCase = new UpdatePriorityUseCase(_repoMock.Object, _mapperMock.Object, _validatorMock.Object, loggerMock.Object);
    }

    [Fact]
    public async Task UpdatePriority_DeveRetornarOk_QuandoDadosValidos()
    {
        var dto = new UpdatePriorityDto { Priority = "Alta" };
        var usuario = new Usuario { Id = 5, Nome = "Teste", Email = "teste@gmail.com", SenhaHash = "Teste" };

        var taskEntity = new TaskE
        {
            Id = 1,
            Usuario = usuario,
            Priority = "Media"
        };

        _validatorMock.Setup(v => v.Validate(dto))
            .Returns(new ValidationResult(new List<ValidationFailure>()));

        _repoMock.Setup(r => r.GetId(taskEntity.Id, usuario.Id))
            .ReturnsAsync(taskEntity);
        
        _repoMock.Setup(r => r.EditTask(taskEntity))
            .Returns(Task.CompletedTask);

        var result = await _useCase.UpdatePriorityAsync(dto, taskEntity.Id,usuario.Id);

        Assert.True(result.IsSuccess);
    }
    
    [Fact]
    public async Task UpdatePriorityAsync_DeveRetornarErro_QuandoValidacaoFalhar()
    {
        var dto = new UpdatePriorityDto { Priority = "" }; // inválido
        var usuarioId = 5;

        var validationResult = new ValidationResult(new List<ValidationFailure>
        {
            new ValidationFailure("Priority", "Prioridade é obrigatória")
        });

        _validatorMock.Setup(v => v.Validate(dto))
            .Returns(validationResult);

        var result = await _useCase.UpdatePriorityAsync(dto, 1, usuarioId);

        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, e => e.Message == "Prioridade é obrigatória" || e.Message == "Erro de validacao");
    }
    
    [Fact]
    public async Task UpdatePriorityAsync_DeveRetornarErro_QuandoExcecaoForLancada()
    {
        var dto = new UpdatePriorityDto { Priority = "Alta" };
        var usuarioId = 5;

        _validatorMock.Setup(v => v.Validate(dto))
            .Returns(new ValidationResult(new List<ValidationFailure>()));

        _repoMock.Setup(r => r.GetId(It.IsAny<int>(), It.IsAny<int>()))
            .ThrowsAsync(new Exception("Falha no banco"));

        var result = await _useCase.UpdatePriorityAsync(dto, 1, usuarioId);

        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, e => e.Message.Contains("Erro ao atualizar prioridade"));
    }
    
}