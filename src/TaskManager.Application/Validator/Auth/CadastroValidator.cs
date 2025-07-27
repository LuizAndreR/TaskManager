using FluentValidation;
using TaskManager.Application.Dtos.Auth;

namespace TaskManager.Application.Validator.Auth;

public class CadastroValidator : AbstractValidator<CadastroUsuarioRequest>
{
    public CadastroValidator()
    {
        RuleFor(u => u.Nome)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("O nome e obrigatorio")
            .MinimumLength(3).WithMessage("Nome deve ter no mínimo 3 letras");

        RuleFor(x => x.Email)
           .Cascade(CascadeMode.Stop)
           .NotEmpty().WithMessage("O e-mail é obrigatório")
           .EmailAddress().WithMessage("E-mail inválido");

        RuleFor(x => x.Senha)
            .MinimumLength(8).WithMessage("Senha muito curta")
            .Must(s => s.Any(char.IsUpper)).WithMessage("A senha deve conter pelo menos uma letra maiúscula")
            .Must(s => s.Any(char.IsLower)).WithMessage("A senha deve conter pelo menos uma letra minúscula")
            .Must(s => s.Any(char.IsDigit)).WithMessage("A senha deve conter pelo menos um número")
            .Must(s => s.Any(c => !char.IsLetterOrDigit(c))).WithMessage("A senha deve conter pelo menos um caractere especial");
    }
}
