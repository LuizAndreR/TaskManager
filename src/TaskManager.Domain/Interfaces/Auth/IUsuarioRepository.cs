using TaskManager.Domain.Entities;

namespace TaskManager.Domain.Interfaces.Auth;

public interface IUsuarioRepository
{
    public Task<Usuario?> BuscaUsuario(string email);
    public Task<bool> EmailExiste(string email);
    public Task<Usuario> Salve(Usuario usuario);
}
