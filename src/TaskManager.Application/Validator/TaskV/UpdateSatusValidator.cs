using FluentValidation;
using TaskManager.Application.Dtos.TaskDto;

namespace TaskManager.Application.Validator.TaskV;

public class UpdateSatusValidator : AbstractValidator<UpdateStatusTaskDto>
{
    public UpdateSatusValidator()
    {
        RuleFor(t => t.Status)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("O status é obrigatório.")
            .Must(BeAValidStatus).WithMessage("O status deve ser: Pendente, EmAndamento ou Concluida.");;
    }
    private bool BeAValidStatus(string status)
    {
        var valid = new[] { "Pendente", "EmAndamento", "Concluida" };
        return valid.Contains(status, StringComparer.OrdinalIgnoreCase);
    }
}