using AutoMapper;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using TaskManager.Application.Dtos.TaskDto;
using TaskManager.Application.UseCase.Task;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Interfaces.ITask;

namespace TaskManager.Tests.UseCase;

public class UseCaseTaskTests
{
    private readonly Mock<ITaskRepository> _repoMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILogger<UseCaseTask>> _loggerMock;
    private readonly Mock<IValidator<CreateTaskDto>> _validatorMock;
    private readonly UseCaseTask _useCase;

    public UseCaseTaskTests()
    {
        _repoMock = new Mock<ITaskRepository>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<UseCaseTask>>();
        _validatorMock = new Mock<IValidator<CreateTaskDto>>();

        _useCase = new UseCaseTask(_repoMock.Object, _mapperMock.Object, _validatorMock.Object, _loggerMock.Object);
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

        var result = await _useCase.BuscaTasksbyIdUserAsync(userId);

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

        var result = await _useCase.BuscaTasksbyIdUserAsync(userId);

        Assert.True(result.IsFailed);
        Assert.Contains("Nenhuma tarefa encontrada", result.Errors.First().Message);
    }

    [Fact]
    public async Task Deve_Retornar_Falha_Quando_Exception_For_Lancada()
    {
        var userId = 1;

        _repoMock.Setup(r => r.GetByUserIdAsync(userId)).ThrowsAsync(new Exception("Erro teste"));

        var result = await _useCase.BuscaTasksbyIdUserAsync(userId);

        Assert.True(result.IsFailed);
        Assert.Contains("Erro interno inesperado", result.Errors.First().Message);
    }

    // Testes Get por Id

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

        var result = await _useCase.BuscaTaskbyIdAsync(tarefa.Id, userId);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(tarefaDto);
    }

    [Fact]
    public async Task BuscaTaskbyIdAsync_DeveRetornarErroQuandoNaoEncontrada()
    {
        var id = 2;
        var userId = 456;

        _repoMock.Setup(r => r.GetId(id, userId)).ReturnsAsync((TaskE?)null);

        var result = await _useCase.BuscaTaskbyIdAsync(id, userId);

        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Be("Nenhuma tarefa encontrada com id");
    }

    [Fact]
    public async Task BuscaTaskbyIdAsync_DeveRetornarErroQuandoOcorrerExcecao()
    {
        var id = 3;
        var userId = 789;

        _repoMock.Setup(r => r.GetId(id, userId)).ThrowsAsync(new Exception("Erro de banco de dados"));

        var result = await _useCase.BuscaTaskbyIdAsync(id, userId);

        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Contain("Erro interno inesperado");
    }

    // Teste Create

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
