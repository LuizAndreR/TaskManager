using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Interfaces.Auth;
using TaskManager.Infrastructure.Data.Context;

namespace TaskManager.Infrastructure.Data.Repositories.Auth;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly TaskManagerContext _context;
    private readonly ILogger<UsuarioRepository> _logger;

    public UsuarioRepository(TaskManagerContext context, ILogger<UsuarioRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Usuario?> BuscaUsuario(string email)
    {
        _logger.LogInformation("Buscando usuário com email: {Email}", email);
        return await _context.Usuarios.SingleOrDefaultAsync(u => u.Email == email);
    }

    public async Task<bool> EmailExiste(string email)
    {
        _logger.LogInformation("Verificando existência do email no banco de dados: {Email}", email);
        return await _context.Usuarios.AnyAsync(u => u.Email == email);
    }

    public async Task Salve(Usuario usuario)
    {
        _logger.LogInformation("Salvando novo usuário no banco de dados: {Email}", usuario.Email);
        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();
    }
}
