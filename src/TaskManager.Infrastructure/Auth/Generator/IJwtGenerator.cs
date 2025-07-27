namespace TaskManager.Infrastructure.Auth.Generator;

public interface IJwtGenerator
{
    public string Generate(string email);
}
