using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using TaskManager.Domain.Entities;
using TaskManager.Infrastructure.Data;
using TaskManager.Infrastructure.Data.Repositories.Auth;

namespace TaskManager.Tests.Repository;

public class UsuarioRepositoryTest
{
    private DbContextOptions<TaskManagerContext> _options;
    private Mock<ILogger<UsuarioRepository>> _logger;
    
    public UsuarioRepositoryTest()
    {
        _logger = new Mock<ILogger<UsuarioRepository>>();

        _options = new DbContextOptionsBuilder<TaskManagerContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public async Task EmailExiste_DeveRetornarTrue_QuandoEmailJaExiste()
    {
        using var context = new TaskManagerContext(_options);
        context.Usuarios.Add(new Usuario { Nome = "Teste", Email = "teste@email.com", SenhaHash = "123" });
        context.SaveChanges();

        var repo = new UsuarioRepository(context, _logger.Object);

        var existe = await repo.EmailExiste("teste@email.com");

        Assert.True(existe);
    }

    [Fact]
    public async Task EmailNaoExiste_DeveRetornarFalse_QuandoEmailNaoExiste()
    {
        using var context = new TaskManagerContext(_options);
        context.Usuarios.Add(new Usuario { Nome = "Teste", Email = "teste@email.com", SenhaHash = "123" });
        context.SaveChanges();

        var repo = new UsuarioRepository(context, _logger.Object);

        var existe = await repo.EmailExiste("teste1@email.com");

        Assert.False(existe);
    }

    [Fact]
    public async Task Salva_DeveSalvarUsuario()
    {
        using var context = new TaskManagerContext(_options);

        var repo = new UsuarioRepository(context, _logger.Object);

        var novoUsuario = new Usuario { Nome = "Teste", Email = "teste@email.com", SenhaHash = "123" };

        await repo.Salve(novoUsuario);

        Assert.Single(context.Usuarios);
        Assert.Equal("teste@email.com", context.Usuarios.First().Email);
    }

    [Fact]
    public async Task BuscaUsuario_DeveRetornarUsuario_QuandoEmailExistir()
    {
        using var context = new TaskManagerContext(_options);
        context.Usuarios.Add(new Usuario { Nome = "Teste", Email = "teste@email.com", SenhaHash = "123" });
        context.SaveChanges();

        var repo = new UsuarioRepository(context, _logger.Object);

        var usuario = await repo.BuscaUsuario("teste@email.com");

        Assert.NotNull(usuario);
        Assert.Equal("teste@email.com", usuario.Email);
    }

    [Fact]
    public async Task BuscaUsuario_DeveRetornarNull_QuandoEmailNaoExistir()
    {
        using var context = new TaskManagerContext(_options);
        context.Usuarios.Add(new Usuario { Nome = "Teste", Email = "teste@email.com", SenhaHash = "123" });
        context.SaveChanges();

        var repo = new UsuarioRepository(context, _logger.Object);

        var usuario = await repo.BuscaUsuario("naoexiste@email.com");

        Assert.Null(usuario);
    }
}
