namespace TaskManager.Application.Dtos.Auth;

public class CadastroUsuarioRequest
{
    public required string Nome { get; set; }
    public required string Email { get; set; }
    public required string Senha { get; set; }
}
