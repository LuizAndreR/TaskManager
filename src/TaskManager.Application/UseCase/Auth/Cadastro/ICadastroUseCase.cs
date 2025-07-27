using FluentResults;
using TaskManager.Application.Dtos.Auth;

namespace TaskManager.Application.UseCase.Auth.Cadastro;

public interface ICadastroUseCase
{
    Task<Result<string>> Execute(CadastroUsuarioRequest request);
}
