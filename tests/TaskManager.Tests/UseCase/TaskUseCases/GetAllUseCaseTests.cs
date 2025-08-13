using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using TaskManager.Application.Dtos.TaskDto;
using TaskManager.Application.UseCase.Tasks.UseCases;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Interfaces.ITask;

namespace TaskManager.Tests.UseCase.TaskUseCases;

public class GetAllUseCaseTests
{
    private readonly Mock<ITaskRepository> _repoMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetAllTasksUseCase _useCase;

    public GetAllUseCaseTests()
    {
        _repoMock = new Mock<ITaskRepository>();
        _mapperMock = new Mock<IMapper>();
        var loggerMock = new Mock<ILogger<GetAllTasksUseCase>>();

        _useCase = new GetAllTasksUseCase(_repoMock.Object, _mapperMock.Object, loggerMock.Object);
    }

    // Teste do GetAll

    [Fact]
    public async Task Deve_Retornar_Sucesso_Quando_Tarefas_Existirem()
    {
        var userId = 1;
        var tarefas = new List<TaskE>
        {
            new TaskE
            {
                Id = 1,
                Title = "Tarefa1",
                UsuarioId = userId,
                Priority = "Média",
                Status = "Pendente",
                DateCreated = DateTime.UtcNow,
                Usuario = new Usuario { Id = userId, Nome = "Teste", Email = "teste@email.com", SenhaHash = "123456" }
            }
        };
        var tarefasDto = new List<GetTaskDto>
        {
            new GetTaskDto
            {
                Title = "Tarefa1",
                Priority = "Média",
                Status = "Pendente",
                DateCreated = tarefas[0].DateCreated,
                Descriptions = null
            }
        };
        _repoMock.Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync(tarefas);
        _mapperMock.Setup(m => m.Map<IEnumerable<GetTaskDto>>(tarefas)).Returns(tarefasDto);

        var result = await _useCase.GetAllTasksAsync(userId);

        Assert.True(result.IsSuccess);
        Assert.NotEmpty(result.Value);

        var dto = result.Value.First();
        Assert.Equal("Tarefa1", dto.Title);
        Assert.Equal("Média", dto.Priority);
        Assert.Equal("Pendente", dto.Status);
        Assert.Equal(tarefas[0].DateCreated, dto.DateCreated);
    }


    [Fact]
    public async Task Deve_Retornar_Falha_Quando_Nao_Houver_Tarefas()
    {
        var userId = 1;

        _repoMock.Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync(new List<TaskE>());

        var result = await _useCase.GetAllTasksAsync(userId);

        Assert.True(result.IsFailed);
        Assert.Contains("Nenhuma tarefa encontrada", result.Errors.First().Message);
    }

    [Fact]
    public async Task Deve_Retornar_Falha_Quando_Exception_For_Lancada()
    {
        var userId = 1;

        _repoMock.Setup(r => r.GetByUserIdAsync(userId)).ThrowsAsync(new Exception("Erro teste"));

        var result = await _useCase.GetAllTasksAsync(userId);

        Assert.True(result.IsFailed);
        Assert.Contains("Erro interno inesperado", result.Errors.First().Message);
    }
}