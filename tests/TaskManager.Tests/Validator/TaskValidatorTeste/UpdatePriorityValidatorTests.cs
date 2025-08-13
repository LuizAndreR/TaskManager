using FluentValidation;
using FluentValidation.TestHelper;
using TaskManager.Application.Dtos.TaskDto;
using TaskManager.Application.Validator.TaskV;

namespace TaskManager.Tests.Validator;

public class UpdatePriorityValidatorTests
{
    private readonly UpdatePriorityValidator _validator;

    public UpdatePriorityValidatorTests()
    {
        _validator =  new UpdatePriorityValidator();
    }

    [Fact]
    public void UpdatePriority_Valid()
    {
        var dto = new UpdatePriorityDto
        {
            Priority = "Media"
        };

        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void UpdatePriority_Invalid_Priority_Null()
    {
        var dto = new UpdatePriorityDto
        {
            Priority = ""
        };

        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Priority)
            .WithErrorMessage("A prioridade é obrigatória.");
    }

    [Fact]
    public void UpdatePriority_Invalid_Priority_Invalid()
    {
        var dto = new UpdatePriorityDto
        {
            Priority = "invalid"
        };
        
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Priority)
            .WithErrorMessage("A prioridade deve ser: Baixa, Media ou Alta.");
    }
}
