using FluentValidation;
using TaskManager.Application.Dtos.TaskDto;

namespace TaskManager.Application.Validator.TaskV;

public class CreateTaskValidator : AbstractValidator<CreateTaskDto>
{
    public CreateTaskValidator()
    {
        RuleFor(t => t.Title)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("O Title e obrigatorio")
            .MaximumLength(100).WithMessage("O título deve ter no máximo 100 caracteres.");

        RuleFor(t => t.Descriptions)
            .MaximumLength(500).WithMessage("A descrição deve ter no máximo 500 caracteres.");

         RuleFor(x => x.Priority)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("A prioridade é obrigatória.")
            .Must(BeAValidPriority).WithMessage("A prioridade deve ser: Baixa, Media ou Alta.");

        RuleFor(x => x.Status)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("O status é obrigatório.")
            .Must(BeAValidStatus).WithMessage("O status deve ser: Pendente, EmAndamento ou Concluida.");
    }

    private bool BeAValidPriority(string priority)
    {
        var valid = new[] { "Baixa", "Media", "Alta" };
        return valid.Contains(priority, StringComparer.OrdinalIgnoreCase);
    }

    private bool BeAValidStatus(string status)
    {
        var valid = new[] { "Pendente", "EmAndamento", "Concluida" };
        return valid.Contains(status, StringComparer.OrdinalIgnoreCase);
    }
}
