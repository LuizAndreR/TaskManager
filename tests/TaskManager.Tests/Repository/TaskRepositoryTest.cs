using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using TaskManager.Infrastructure.Data;
using TaskManager.Domain.Entities;
using TaskManager.Infrastructure.Data.Repositories.Task;

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
}
