using FluentValidation;
using FluentValidation.TestHelper;
using TaskManager.Application.Dtos.TaskDto;
using TaskManager.Application.Validator.TaskV;

namespace TaskManager.Tests.Validator;

public class UpdateStatusValidatorTests
{
    private readonly UpdateStatusValidator _validator;

    public UpdateStatusValidatorTests()
    {
        _validator =  new UpdateStatusValidator();
    }

    [Fact]
    public void UpdatePriority_Valid()
    {
        var dto = new UpdateStatusTaskDto
        {
            Status = "EmAndamento"
        };

        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void UpdatePriority_Invalid_Priority_Null()
    {
        var dto = new UpdateStatusTaskDto()
        {
            Status = ""
        };

        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Status)
            .WithErrorMessage("O status é obrigatório.");
    }

    [Fact]
    public void UpdatePriority_Invalid_Priority_Invalid()
    {
        var dto = new UpdateStatusTaskDto()
        {
            Status = "invalid"
        };
        
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Status)
            .WithErrorMessage("O status deve ser: Pendente, EmAndamento ou Concluida.");
    }
}