using TaskManager.Application.Dtos.TaskDto;
using TaskManager.Application.Validator.TaskV;
using FluentValidation.TestHelper;

namespace TaskManager.Tests.Validator;

public class CreateTaskValidatorTest
{
    private readonly CreateTaskValidator _validator;

    public CreateTaskValidatorTest()
    {
        _validator = new CreateTaskValidator();
    }

    [Fact]
    public void Deve_Passar_Quando_Dados_Sao_Validos()
    {
        var dto = new CreateTaskDto
        {
            Title = "Tarefa válida",
            Descriptions = "Descrição qualquer",
            Priority = "Alta",
            Status = "Pendente"
        };

        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Deve_Falhar_Quando_Title_Estiver_Vazio()
    {
        var dto = new CreateTaskDto
        {
            Title = "",
            Descriptions = "Alguma descrição",
            Priority = "Alta",
            Status = "Pendente"
        };

        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Title)
              .WithErrorMessage("O Title e obrigatorio");
    }

    [Fact]
    public void Deve_Falhar_Quando_Title_For_Muito_Grande()
    {
        var dto = new CreateTaskDto
        {
            Title = new string('A', 101),
            Descriptions = "Descrição normal",
            Priority = "Alta",
            Status = "Pendente"
        };

        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Title)
              .WithErrorMessage("O título deve ter no máximo 100 caracteres.");
    }

    [Fact]
    public void Deve_Falhar_Quando_Descriptions_For_Muito_Grande()
    {
        var dto = new CreateTaskDto
        {
            Title = "Tarefa",
            Descriptions = new string('B', 501),
            Priority = "Alta",
            Status = "Pendente"
        };

        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Descriptions)
              .WithErrorMessage("A descrição deve ter no máximo 500 caracteres.");
    }

    [Fact]
    public void Deve_Falhar_Quando_Priority_For_Invalida()
    {
        var dto = new CreateTaskDto
        {
            Title = "Tarefa",
            Descriptions = "Desc",
            Priority = "Urgente",
            Status = "Pendente"
        };

        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Priority)
              .WithErrorMessage("A prioridade deve ser: Baixa, Media ou Alta.");
    }

    [Fact]
    public void Deve_Falhar_Quando_Status_For_Invalido()
    {
        var dto = new CreateTaskDto
        {
            Title = "Tarefa",
            Descriptions = "Desc",
            Priority = "Alta",
            Status = "Cancelado"
        };

        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Status)
              .WithErrorMessage("O status deve ser: Pendente, EmAndamento ou Concluida.");
    }

    [Fact]
    public void Deve_Falhar_Quando_Priority_E_Status_Estiverem_Vazios()
    {
        var dto = new CreateTaskDto
        {
            Title = "Tarefa",
            Descriptions = "Desc",
            Priority = "",
            Status = ""
        };

        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Priority)
              .WithErrorMessage("A prioridade é obrigatória.");

        result.ShouldHaveValidationErrorFor(x => x.Status)
              .WithErrorMessage("O status é obrigatório.");
    }
}
