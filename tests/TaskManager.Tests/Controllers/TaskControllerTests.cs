using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;
using TaskManager.Api.Controllers.Task;
using TaskManager.Application.Dtos.TaskDto;
using TaskManager.Application.UseCase.Task;

namespace TaskManager.Tests.Controllers;

public class TaskControllerTests
{
    private TaskController CriarControllerComUsuario(Result<int> userIdResult,Result<IEnumerable<GetTaskDto>>? tasksResult = null,Result<GetTaskDto>? taskByIdResult = null)
    {
        var useCaseMock = new Mock<IUseCaseTask>();

        if (tasksResult != null)
        {
            useCaseMock
                .Setup(x => x.BuscaTasksbyIdUserAsync(It.IsAny<int>()))
                .ReturnsAsync(tasksResult);
        }

        if (taskByIdResult != null)
        {
            useCaseMock
                .Setup(x => x.BuscaTaskbyIdAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(taskByIdResult);
        }

        var loggerMock = new Mock<ILogger<TaskController>>();

        var controller = new TaskController(useCaseMock.Object, loggerMock.Object);

        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userIdResult.IsSuccess ? userIdResult.Value.ToString() : string.Empty)
        }));

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };

        return controller;
    }

    [Fact]
    public async Task Home_DeveRetornarOk_QuandoTarefasForemEncontradas()
    {
        var tarefas = new List<GetTaskDto>
        {       
            new GetTaskDto { Title = "Tarefa 1", Priority = "Alta", Status = "Aberta", DateCreated = DateTime.Now },
            new GetTaskDto { Title = "Tarefa 2", Priority = "Média", Status = "Fechada", DateCreated = DateTime.Now }
        };

        var controller = CriarControllerComUsuario(Result.Ok(123), Result.Ok<IEnumerable<GetTaskDto>>(tarefas));

        var resultado = await controller.GetAll();

        var okResult = Assert.IsType<OkObjectResult>(resultado);
        var retorno = Assert.IsAssignableFrom<IEnumerable<GetTaskDto>>(okResult.Value);
        Assert.Equal(2, retorno.Count());
    }

    [Fact]
    public async Task Home_DeveRetornarUnauthorized_QuandoUsuarioNaoAutenticado()
    {
        var controller = CriarControllerComUsuario(Result.Fail("Usuário não autenticado"), Result.Fail<IEnumerable<GetTaskDto>>("Não importa"));

        var resultado = await controller.GetAll();

        Assert.IsType<UnauthorizedResult>(resultado);
    }

    [Fact]
    public async Task Home_DeveRetornarNotFound_QuandoNenhumaTarefaForEncontrada()
    {
        var controller = CriarControllerComUsuario(
            Result.Ok(123),
            Result.Fail<IEnumerable<GetTaskDto>>("Nenhuma tarefa encontrada para o usuário")
        );

        var resultado = await controller.GetAll();

        var notFound = Assert.IsType<NotFoundObjectResult>(resultado);
        var mensagem = notFound.Value?.GetType().GetProperty("message")?.GetValue(notFound.Value)?.ToString();

        Assert.Equal("Nenhuma tarefa encontrada para o usuário", mensagem);
    }

    [Fact]
    public async Task Home_DeveRetornarErro500_QuandoErroNaoForDeTarefaNaoEncontrada()
    {
        var controller = CriarControllerComUsuario(
            Result.Ok(123),
            Result.Fail<IEnumerable<GetTaskDto>>("Erro inesperado no banco de dados")
        );

        var resultado = await controller.GetAll();

        var result = Assert.IsType<ObjectResult>(resultado);
        Assert.Equal(500, result.StatusCode);
    }

    // Testes para Get por Id

    [Fact]
    public async Task GetId_DeveRetornarOk_QuandoTarefaForEncontrada()
    {
        var tarefaDto = new GetTaskDto
        {
            Title = "Tarefa 1",
            Priority = "Alta",
            Status = "Pendente",
            DateCreated = DateTime.UtcNow
        };

        var controller = CriarControllerComUsuario(
            Result.Ok(123),
            taskByIdResult: Result.Ok(tarefaDto));

        var resultado = await controller.GetId(1);

        var okResult = Assert.IsType<OkObjectResult>(resultado);
        var retorno = Assert.IsType<GetTaskDto>(okResult.Value);
        Assert.Equal("Tarefa 1", retorno.Title);
    }

    [Fact]
    public async Task GetId_DeveRetornarNotFound_QuandoTarefaNaoForEncontrada()
    {
        var controller = CriarControllerComUsuario(
            userIdResult: Result.Ok(123),
            tasksResult: null, 
            taskByIdResult: Result.Fail<GetTaskDto>("Nenhuma tarefa encontrada com id")
        );

        var resultado = await controller.GetId(1);

        var notFound = Assert.IsType<NotFoundObjectResult>(resultado);
        var mensagem = notFound.Value?.GetType().GetProperty("message")?.GetValue(notFound.Value)?.ToString();

        Assert.Equal("Nenhuma tarefa encontrada com id", mensagem);
    }

    [Fact]
    public async Task GetId_DeveRetornarUnauthorized_QuandoUsuarioNaoAutenticado()
    {
        var loggerMock = new Mock<ILogger<TaskController>>();
        var useCaseMock = new Mock<IUseCaseTask>();
        var controller = new TaskController(useCaseMock.Object, loggerMock.Object);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext() 
        };

        var result = await controller.GetId(1); 

        Assert.IsType<UnauthorizedResult>(result);
    }


    [Fact]
    public async Task GetId_DeveRetornarErro500_QuandoErroForInesperado()
    {
        var controller = CriarControllerComUsuario(
            userIdResult: Result.Ok(123),
            tasksResult: null,
            taskByIdResult: Result.Fail<GetTaskDto>("Erro inesperado ao buscar tarefa")
        );

        var resultado = await controller.GetId(1);

        var objectResult = Assert.IsType<ObjectResult>(resultado);
        Assert.Equal(500, objectResult.StatusCode);
        var mensagem = objectResult.Value?.GetType().GetProperty("message")?.GetValue(objectResult.Value)?.ToString();

        Assert.Equal("Erro interno inesperado ao buscar tarefas.", mensagem);
    }

    // Testes endpoint post

    [Fact]
    public async Task Create_DeveRetornarCreated_QuandoCriacaoForBemSucedida()
    {
        var controllerLogger = new Mock<ILogger<TaskController>>();
        var useCaseMock = new Mock<IUseCaseTask>();
        var request = new CreateTaskDto
        {
            Title = "Nova tarefa",
            Descriptions = "Descrição da tarefa",
            Priority = "Alta",
            Status = "Pendente"
        };

        var expectedDto = new GetTaskDto
        {
            Id = 1,
            Title = request.Title,
            Descriptions = request.Descriptions,
            Priority = request.Priority,
            Status = request.Status,
        };

        useCaseMock.Setup(u => u.CreateTaskAsync(request, 5))
            .ReturnsAsync(Result.Ok(expectedDto));

        var controller = new TaskController(useCaseMock.Object, controllerLogger.Object);

        var httpContext = new DefaultHttpContext();
        var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, "5") };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        httpContext.User = new ClaimsPrincipal(identity);
        controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

        var result = await controller.Create(request);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        var returnValue = Assert.IsType<GetTaskDto>(createdResult.Value);
        Assert.Equal(expectedDto.Id, returnValue.Id);
        Assert.Equal(201, createdResult.StatusCode);
    }

    [Fact]
    public async Task Create_DeveRetornarBadRequest_QuandoHouverErroDeValidacao()
    {
        var controllerLogger = new Mock<ILogger<TaskController>>();
        var useCaseMock = new Mock<IUseCaseTask>();

        var request = new CreateTaskDto
        {
            Title = "", 
            Descriptions = "Descrição inválida",
            Priority = "Alta",
            Status = "Pendente"
        };

        var validationError = new Error("Erro de validação: Título é obrigatório");

        useCaseMock.Setup(u => u.CreateTaskAsync(request, 5))
            .ReturnsAsync(Result.Fail<GetTaskDto>(validationError));

        var controller = new TaskController(useCaseMock.Object, controllerLogger.Object);

        var httpContext = new DefaultHttpContext();
        var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, "5") };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        httpContext.User = new ClaimsPrincipal(identity);
        controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

        var result = await controller.Create(request);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        var errorMessages = Assert.IsAssignableFrom<IEnumerable<string>>(badRequest.Value);
        Assert.Contains("Erro de validação", errorMessages.First());
        Assert.Equal(400, badRequest.StatusCode);
    }

    [Fact]
    public async Task Create_DeveRetornarUnauthorized_QuandoUsuarioNaoAutenticado()
    {
        var controllerLogger = new Mock<ILogger<TaskController>>();
        var useCaseMock = new Mock<IUseCaseTask>();

        var controller = new TaskController(useCaseMock.Object, controllerLogger.Object);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext() 
        };

        var request = new CreateTaskDto
        {
            Title = "Tarefa sem token",
            Descriptions = "Descrição",
            Priority = "Média",
            Status = "Pendente"
        };

        var result = await controller.Create(request);

        Assert.IsType<UnauthorizedResult>(result);
    }


    [Fact]
    public async Task Create_DeveRetornarErro500_QuandoErroInesperadoAcontecer()
    {
        var controllerLogger = new Mock<ILogger<TaskController>>();
        var useCaseMock = new Mock<IUseCaseTask>();

        var request = new CreateTaskDto
        {
            Title = "Tarefa com erro",
            Descriptions = "Erro inesperado",
            Priority = "Alta",
            Status = "Pendente"
        };

        var erroInterno = new Error("Erro inesperado: banco de dados indisponível");

        useCaseMock.Setup(u => u.CreateTaskAsync(request, 5))
            .ReturnsAsync(Result.Fail<GetTaskDto>(erroInterno));

        var controller = new TaskController(useCaseMock.Object, controllerLogger.Object);

        var httpContext = new DefaultHttpContext();
        var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, "5") };
        httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(claims));
        controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

        var result = await controller.Create(request);

        var statusCode = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusCode.StatusCode);
        Assert.Equal("Erro inesperado: banco de dados indisponível", statusCode.Value);
    }

}
