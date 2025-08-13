using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using TaskManager.Infrastructure.Data;
using TaskManager.Domain.Entities;
using TaskManager.Infrastructure.Data.Repositories.TaskR;

namespace TaskManager.Tests.Repository;

public class TaskRepositoryTest
{
    private DbContextOptions<TaskManagerContext> _options;
    private Mock<ILogger<TaskRepository>> _logger;

    public TaskRepositoryTest()
    {
        _logger = new Mock<ILogger<TaskRepository>>();

        _options = new DbContextOptionsBuilder<TaskManagerContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public async Task Deve_Retornar_Tarefas_Do_Usuario()
    {
        using var context = new TaskManagerContext(_options);

        var userId = 1;
        context.Tarefas.AddRange(
            new TaskE { Id = 1, Title = "Tarefa 1", Priority = "Media", Status = "EmAndamento", UsuarioId = userId, Usuario = new Usuario { Id = userId, Nome = "Teste", Email = "teste@email.com", SenhaHash = "123456" } },
            new TaskE { Id = 2, Title = "Tarefa 2", Priority = "Media", Status = "EmAndamento", UsuarioId = 2, Usuario = new Usuario { Id = 2, Nome = "Teste2", Email = "outro@email.com", SenhaHash = "123456" } }
        );
        await context.SaveChangesAsync();

        var repository = new TaskRepository(context, _logger.Object);

        var result = await repository.GetByUserIdAsync(userId);

        Assert.NotNull(result);
        var list = result.ToList();
        Assert.Single(list);
        Assert.Equal("Tarefa 1", list[0].Title);
        Assert.Equal(userId, list[0].UsuarioId);
    }
    [Fact]
    public async Task GetId_DeveRetornarTarefaQuandoExistir()
    {
        using var context = new TaskManagerContext(_options);
        var repository = new TaskRepository(context, _logger.Object);

        var usuario = new Usuario
        {
            Id = 1,
            Nome = "Usuário Teste",
            Email = "usuario1@teste.com",
            SenhaHash = "senha123"
        };

        var tarefa = new TaskE
        {
            Id = 1,
            Title = "Tarefa Teste",
            Descriptions = "Descrição",
            Priority = "Alta",
            Status = "Pendente",
            UsuarioId = usuario.Id,
            Usuario = usuario
        };

        context.Usuarios.Add(usuario);
        context.Tarefas.Add(tarefa);
        await context.SaveChangesAsync();

        var resultado = await repository.GetId(1, 1);

        Assert.NotNull(resultado);
        Assert.Equal("Tarefa Teste", resultado!.Title);
    }

    [Fact]
    public async Task CreateTask_DeveAdicionarTarefa()
    {
        using var context = new TaskManagerContext(_options);
        var repository = new TaskRepository(context, _logger.Object);

        var usuario = new Usuario
        {
            Id = 2,
            Nome = "Usuário 2",
            Email = "usuario2@teste.com",
            SenhaHash = "senha456"
        };

        var novaTarefa = new TaskE
        {
            Id = 2,
            Title = "Nova Tarefa",
            Descriptions = "Descrição",
            Priority = "Média",
            Status = "Em Andamento",
            UsuarioId = usuario.Id,
            Usuario = usuario
        };

        context.Usuarios.Add(usuario);

        var resultado = await repository.CreateTask(novaTarefa);

        var tarefaNoBanco = await context.Tarefas.FindAsync(2);
        Assert.NotNull(tarefaNoBanco);
        Assert.Equal("Nova Tarefa", tarefaNoBanco!.Title);
        Assert.Equal(resultado.Id, tarefaNoBanco.Id);
    }

    [Fact]
    public async Task EditTask_DeveEditarTarefaExistente()
    {
        using var context = new TaskManagerContext(_options);
        var repository = new TaskRepository(context, _logger.Object);

        var usuario = new Usuario
        {
            Id = 3,
            Nome = "Usuário 3",
            Email = "usuario3@teste.com",
            SenhaHash = "senha789"
        };

        var tarefa = new TaskE
        {
            Id = 3,
            Title = "Título Antigo",
            Descriptions = "Antiga descrição",
            Priority = "Baixa",
            Status = "Pendente",
            UsuarioId = usuario.Id,
            Usuario = usuario
        };

        context.Usuarios.Add(usuario);
        context.Tarefas.Add(tarefa);
        await context.SaveChangesAsync();

        tarefa.Title = "Título Atualizado";
        tarefa.Status = "Concluída";

        await repository.EditTask(tarefa);

        var tarefaAtualizada = await context.Tarefas.FindAsync(3);
        Assert.NotNull(tarefaAtualizada);
        Assert.Equal("Título Atualizado", tarefaAtualizada!.Title);
        Assert.Equal("Concluída", tarefaAtualizada.Status);
    }

    [Fact]
    public async Task DeleteTask_DeveExcluirTarefa()
    {
        using var context = new TaskManagerContext(_options);
        var repository = new TaskRepository(context, _logger.Object);
        
        var usuario = new Usuario
        {
            Id = 3,
            Nome = "Usuário 3",
            Email = "usuario3@teste.com",
            SenhaHash = "senha789"
        };

        var tarefa = new TaskE
        {
            Id = 3,
            Title = "Título Antigo",
            Descriptions = "Antiga descrição",
            Priority = "Baixa",
            Status = "Pendente",
            UsuarioId = usuario.Id,
            Usuario = usuario
        };
        
        
        context.Usuarios.Add(usuario);
        context.Tarefas.Add(tarefa);
        await context.SaveChangesAsync();

        await repository.DeleteTask(tarefa, 3);
        
        var tarefaAtualizada = await context.Tarefas.FindAsync(3);
        Assert.Null(tarefaAtualizada);
    }
}
