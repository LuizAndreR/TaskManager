using FluentValidation;
using TaskManager.Application.Dtos.Auth;

namespace TaskManager.Application.Validator.Auth;

public class LoginValidator : AbstractValidator<LoginUsuarioRequest>
{
    public LoginValidator()
    {
        RuleFor(x => x.Email)
          .Cascade(CascadeMode.Stop)
          .NotEmpty().WithMessage("O e-mail é obrigatório")
          .EmailAddress().WithMessage("E-mail inválido");

        RuleFor(x => x.Senha)
            .NotEmpty().WithMessage("A senha é obrigatório");
    }
}
