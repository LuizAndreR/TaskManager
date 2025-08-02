using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using TaskManager.Application.Dtos.TaskDto;
using TaskManager.Application.UseCase.Tasks.UseCases;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Interfaces.ITask;

namespace TaskManager.Tests.UseCase.TaskUseCases;

public class CreateTaskUseCaseTests
{
    private readonly Mock<ITaskRepository> _repoMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IValidator<CreateTaskDto>> _validatorMock;
    private readonly CreateTaskUseCase _useCase;

    public CreateTaskUseCaseTests()
    {
        _repoMock = new Mock<ITaskRepository>();
        _mapperMock = new Mock<IMapper>();
        _validatorMock = new Mock<IValidator<CreateTaskDto>>();
        var loggerMock = new Mock<ILogger<CreateTaskUseCase>>();

        _useCase = new CreateTaskUseCase(_repoMock.Object, _mapperMock.Object, _validatorMock.Object,loggerMock.Object);
    }
    
    [Fact]
    public async Task CreateTaskAsync_DeveRetornarOk_QuandoDadosValidos()
    {
        var dto = new CreateTaskDto { Title = "Tarefa 1", Descriptions = "Desc", Priority = "Alta", Status = "Pendente" };
        var usuario = new Usuario { Id = 5, Nome = "Teste", Email = "teste@gmail.com", SenhaHash = "Teste" };

        var taskEntity = new TaskE
        {
            Id = 1,
            Title = dto.Title,
            Descriptions = dto.Descriptions,
            Priority = dto.Priority,
            Status = dto.Status,
            UsuarioId = 5,
            Usuario = usuario
        };

        var taskDto = new GetTaskDto { Id = 1, Title = dto.Title, Priority = dto.Priority, Status = dto.Status };

        _validatorMock.Setup(v => v.Validate(dto)).Returns(new ValidationResult());
        _mapperMock.Setup(m => m.Map<TaskE>(dto)).Returns(taskEntity);
        _repoMock.Setup(r => r.CreateTask(It.IsAny<TaskE>())).ReturnsAsync(taskEntity);
        _mapperMock.Setup(m => m.Map<GetTaskDto>(taskEntity)).Returns(taskDto);

        var result = await _useCase.CreateTaskAsync(dto, 5);

        Assert.True(result.IsSuccess);
        Assert.Equal(taskDto, result.Value);
    }

    [Fact]
    public async Task CreateTaskAsync_DeveRetornarErro_QuandoDadosInvalidos()
    {
        var dto = new CreateTaskDto { Title = "", Descriptions = "Desc", Priority = "Alta", Status = "Pendente" };
        var validationResult = new ValidationResult(new List<ValidationFailure>
        {
            new ValidationFailure("Title", "Título é obrigatório")
        });

        _validatorMock.Setup(v => v.Validate(dto)).Returns(validationResult);

        var result = await _useCase.CreateTaskAsync(dto, 5);

        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, e => e.Message == "Título é obrigatório");
    }

    [Fact]
    public async Task CreateTaskAsync_DeveRetornarErro_QuandoExcecaoForLancada()
    {
        var dto = new CreateTaskDto { Title = "Tarefa", Descriptions = "Desc", Priority = "Alta", Status = "Pendente" };

        _validatorMock.Setup(v => v.Validate(dto)).Returns(new ValidationResult());
        _mapperMock.Setup(m => m.Map<TaskE>(dto)).Throws(new Exception("Falha"));

        var result = await _useCase.CreateTaskAsync(dto, 5);

        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, e => e.Message.Contains("Falha"));
    }
}