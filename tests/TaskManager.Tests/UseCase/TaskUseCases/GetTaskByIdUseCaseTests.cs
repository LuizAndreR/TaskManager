using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TaskManager.Application.Dtos.TaskDto;
using TaskManager.Application.UseCase.Tasks.UseCases;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Interfaces.ITask;

namespace TaskManager.Tests.UseCase.TaskUseCases;

public class GetTaskByIdUseCaseTests
{
    private readonly Mock<ITaskRepository> _repoMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetTaskByIdUseCase _useCase;

    public GetTaskByIdUseCaseTests()
    {
        _repoMock = new Mock<ITaskRepository>();
        _mapperMock = new Mock<IMapper>();
        var loggerMock = new Mock<ILogger<GetTaskByIdUseCase>>();

        _useCase = new GetTaskByIdUseCase(_repoMock.Object, _mapperMock.Object, loggerMock.Object);
    }
    
    [Fact]
    public async Task BuscaTaskbyIdAsync_DeveRetornarTarefaQuandoEncontrada()
    {
        var userId = 1;
        var tarefa = new TaskE
        {
            Id = 1,
            Title = "Tarefa1",
            UsuarioId = userId,
            Priority = "Média",
            Status = "Pendente",
            DateCreated = DateTime.UtcNow,
            Usuario = new Usuario { Id = userId, Nome = "Teste", Email = "teste@email.com", SenhaHash = "123456" }
        };
        var tarefaDto = new GetTaskDto
        {
            Id = 1,
            Title = "Tarefa1",
            Priority = "Média",
            Status = "Pendente",
            DateCreated = tarefa.DateCreated,
            Descriptions = null
        };

        _repoMock.Setup(r => r.GetId(tarefa.Id, userId)).ReturnsAsync(tarefa);
        _mapperMock.Setup(m => m.Map<GetTaskDto>(tarefa)).Returns(tarefaDto);

        var result = await _useCase.GetTaskByIdAsync(tarefa.Id, userId);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(tarefaDto);
    }

    [Fact]
    public async Task BuscaTaskbyIdAsync_DeveRetornarErroQuandoNaoEncontrada()
    {
        var id = 2;
        var userId = 456;

        _repoMock.Setup(r => r.GetId(id, userId)).ReturnsAsync((TaskE?)null);

        var result = await _useCase.GetTaskByIdAsync(id, userId);

        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Be("Nenhuma tarefa encontrada com id");
    }

    [Fact]
    public async Task BuscaTaskbyIdAsync_DeveRetornarErroQuandoOcorrerExcecao()
    {
        var id = 3;
        var userId = 789;

        _repoMock.Setup(r => r.GetId(id, userId)).ThrowsAsync(new Exception("Erro de banco de dados"));

        var result = await _useCase.GetTaskByIdAsync(id, userId);

        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Contain("Erro interno inesperado");
    }
}