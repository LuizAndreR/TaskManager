namespace TaskManager.Application.Dtos.Auth;

public class LoginUsuarioRequest
{
    public required string Email { get; set; }
    public required string Senha { get; set; }
}
