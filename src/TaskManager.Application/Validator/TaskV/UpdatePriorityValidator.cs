using FluentValidation;
using TaskManager.Application.Dtos.TaskDto;

namespace TaskManager.Application.Validator.TaskV;

public class UpdatePriorityValidator : AbstractValidator<UpdatePriorityDto>
{
    public UpdatePriorityValidator()
    {
        RuleFor(x => x.Priority)
           .Cascade(CascadeMode.Stop)
           .NotEmpty().WithMessage("A prioridade é obrigatória.")
           .Must(BeAValidPriority).WithMessage("A prioridade deve ser: Baixa, Media ou Alta.");
    }

    private bool BeAValidPriority(string priority)
    {
        var valid = new[] { "Baixa", "Media", "Alta" };
        return valid.Contains(priority, StringComparer.OrdinalIgnoreCase);
    }
}
