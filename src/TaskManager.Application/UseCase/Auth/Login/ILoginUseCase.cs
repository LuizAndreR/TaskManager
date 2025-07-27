using FluentResults;
using TaskManager.Application.Dtos.Auth;

namespace TaskManager.Application.UseCase.Auth.Login;

public interface ILoginUseCase
{
    public Task<Result<string>> Execute(LoginUsuarioRequest request);
}
