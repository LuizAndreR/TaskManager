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

public class UpdateTaskUseCaseTests
{
    private readonly Mock<ITaskRepository> _repoMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IValidator<CreateTaskDto>> _validatorMock;
    private readonly UpdateTaskUseCase _useCase;

    public UpdateTaskUseCaseTests()
    {
        _repoMock = new Mock<ITaskRepository>();
        _mapperMock = new Mock<IMapper>();
        _validatorMock = new Mock<IValidator<CreateTaskDto>>();
        var loggerMock = new Mock<ILogger<UpdateTaskUseCase>>();

        _useCase = new UpdateTaskUseCase(_repoMock.Object, _mapperMock.Object, _validatorMock.Object,loggerMock.Object);
    }
    
    [Fact]
    public async Task EditTaskAsync_DeveEditarTarefa_ComSucesso()
    {
        var dto = new CreateTaskDto
        {
            Title = "Título Atualizado",
            Descriptions = "Descrição Atualizada",
            Priority = "Alta",
            Status = "Concluída"
        };

        var tarefa = new TaskE
        {
            Id = 1,
            Title = dto.Title,
            Descriptions = dto.Descriptions,
            Priority = dto.Priority,
            Status = dto.Status,
            UsuarioId = 1,
            Usuario = new Usuario { Id = 1, Nome = "Usuário", Email = "teste@email.com", SenhaHash = "123456" }
        };

        var getDto = new GetTaskDto { Id = 1, Title = dto.Title, Priority = "Alta", Status = "Concluída"};

        _validatorMock.Setup(v => v.Validate(dto)).Returns(new ValidationResult());
        _mapperMock.Setup(m => m.Map<TaskE>(dto)).Returns(tarefa);
        _mapperMock.Setup(m => m.Map<GetTaskDto>(It.IsAny<TaskE>())).Returns(getDto);
        _repoMock.Setup(r => r.EditTask(It.IsAny<TaskE>())).Returns(Task.CompletedTask);

        var result = await _useCase.UpdateTaskAsync(dto, 1, 1);

        Assert.True(result.IsSuccess);
        Assert.Equal("Título Atualizado", result.Value.Title);
    }

    [Fact]
    public async Task EditTaskAsync_DeveRetornarErro_QuandoValidacaoFalha()
    {
        var dto = new CreateTaskDto { Title = "", Priority = "Alta", Status = "Concluída"}; // Inválido

        var erros = new List<ValidationFailure> { new("Title", "Título é obrigatório") };
        var validationResult = new ValidationResult(erros);

        _validatorMock.Setup(v => v.Validate(dto)).Returns(validationResult);
        
        var result = await _useCase.UpdateTaskAsync(dto, 1, 1);

        Assert.False(result.IsSuccess);
        Assert.Contains("Título é obrigatório", result.Errors.Select(e => e.Message));
    }

    [Fact]
    public async Task EditTaskAsync_DeveRetornarErro_QuandoOcorreExcecao()
    {
        var dto = new CreateTaskDto
        {
            Title = "Título",
            Descriptions = "Descrição",
            Priority = "Média",
            Status = "Em andamento"
        };
        
        var tarefa = new TaskE
        {
            Id = 1,
            Title = dto.Title,
            Descriptions = dto.Descriptions,
            Priority = dto.Priority,
            Status = dto.Status,
            UsuarioId = 1,
            Usuario = new Usuario { Id = 1, Nome = "Usuário", Email = "teste@email.com", SenhaHash = "123456" }
        };

        _validatorMock.Setup(v => v.Validate(dto)).Returns(new ValidationResult());
        _mapperMock.Setup(m => m.Map<TaskE>(dto)).Returns(tarefa);
        _repoMock.Setup(r => r.EditTask(It.IsAny<TaskE>())).ThrowsAsync(new Exception("Erro simulado"));

        var result = await _useCase.UpdateTaskAsync(dto, 1, 1);

        Assert.False(result.IsSuccess);
        Assert.Contains("Erro interno inesperado", result.Errors.First().Message);
    }
}