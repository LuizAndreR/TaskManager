using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;
using TaskManager.Api.Controllers.Task;
using TaskManager.Application.Dtos.TaskDto;
using TaskManager.Application.Services;

namespace TaskManager.Tests.Controllers;

public class TaskControllerTests
{
    private TaskController CriarControllerComUsuario(Result<int> userIdResult, Mock<ITaskService> serviceMock)
    {
        var loggerMock = new Mock<ILogger<TaskController>>();

        var controller = new TaskController(serviceMock.Object, loggerMock.Object);

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
    
    // Testes do EndPoint getall
    
    [Fact]
    public async Task GetAll_DeveRetornarOk_QuandoTarefasEncontradas()
    {
        var serviceMock = new Mock<ITaskService>();
        var tarefas = new List<GetTaskDto>
        {
            new() { Id = 1, Title = "Teste", Priority = "Media", Status = "EmAndamento" }
        };

        serviceMock
            .Setup(s => s.GetAllAsync(It.IsAny<int>()))
            .ReturnsAsync(Result.Ok(tarefas.AsEnumerable()));


        var controller = CriarControllerComUsuario(Result.Ok(1), serviceMock);

        var resultado = await controller.GetAll();

        var okResult = Assert.IsType<OkObjectResult>(resultado);
        Assert.Equal(200, okResult.StatusCode);
    }

    [Fact]
    public async Task GetAll_DeveRetornarNotFound_QuandoNaoHouverTarefas()
    {
        var serviceMock = new Mock<ITaskService>();
        serviceMock.Setup(s => s.GetAllAsync(It.IsAny<int>())).ReturnsAsync(Result.Fail("Nenhuma tarefa encontrada"));

        var controller = CriarControllerComUsuario(Result.Ok(1), serviceMock);

        var resultado = await controller.GetAll();

        var notFound = Assert.IsType<NotFoundObjectResult>(resultado);
        Assert.Equal(404, notFound.StatusCode);
    }
    
    [Fact]
    public async Task GetAll_DeveRetornarInternalServerError_QuandoErroInesperado()
    {
        var serviceMock = new Mock<ITaskService>();
    
        serviceMock
            .Setup(s => s.GetAllAsync(It.IsAny<int>()))
            .ReturnsAsync(Result.Fail<IEnumerable<GetTaskDto>>("Erro inesperado no banco de dados"));
    
        var controller = CriarControllerComUsuario(Result.Ok(1), serviceMock);

        var resultado = await controller.GetAll();
        
        var objectResult = Assert.IsType<ObjectResult>(resultado);
        Assert.Equal(500, objectResult.StatusCode);

        var jsonString = System.Text.Json.JsonSerializer.Serialize(objectResult.Value);
        Assert.Contains("Erro interno inesperado ao buscar tarefas.", jsonString);

    }

    [Fact]
    public async Task GetAll_DeveRetornarUnauthorized_QuandoUsuarioNaoAutenticado()
    {
        var userIdResult = Result.Fail<int>("Usuário não autenticado");
        var serviceMock = new Mock<ITaskService>();

        var controller = CriarControllerComUsuario(userIdResult, serviceMock);

        var resultado = await controller.GetAll();

        var unauthorizedResult = Assert.IsType<UnauthorizedResult>(resultado);
        Assert.Equal(401, unauthorizedResult.StatusCode);
    }
    
    // Teste EndPoint getid
    
    [Fact]
    public async Task GetId_DeveRetornarOk_QuandoEncontrarTarefa()
    {
        var serviceMock = new Mock<ITaskService>();
        serviceMock.Setup(s => s.GetByIdAsync(1, 1)).ReturnsAsync(Result.Ok(new GetTaskDto { Id = 1, Title = "Teste",  Priority = "Media", Status = "EmAndamento" }));

        var controller = CriarControllerComUsuario(Result.Ok(1), serviceMock);

        var resultado = await controller.GetId(1);

        var okResult = Assert.IsType<OkObjectResult>(resultado);
        Assert.Equal(200, okResult.StatusCode);
    }

    [Fact]
    public async Task GetId_DeveRetornarNotFound_QuandoNaoEncontrarTarefa()
    {
        var serviceMock = new Mock<ITaskService>();
        serviceMock.Setup(s => s.GetByIdAsync(1, 1)).ReturnsAsync(Result.Fail("Nenhuma tarefa encontrada"));

        var controller = CriarControllerComUsuario(Result.Ok(1), serviceMock);

        var resultado = await controller.GetId(1);

        var notFound = Assert.IsType<NotFoundObjectResult>(resultado);
        Assert.Equal(404, notFound.StatusCode);
    }

    [Fact]
    public async Task GetId_DeveRetornarInternalServerError_QuandoErroInesperado()
    {
        var serviceMock = new Mock<ITaskService>();

        serviceMock
            .Setup(s => s.GetByIdAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(Result.Fail<GetTaskDto>("Erro inesperado no banco de dados"));

        var controller = CriarControllerComUsuario(Result.Ok(1), serviceMock);

        var resultado = await controller.GetId(1);

        var objectResult = Assert.IsType<ObjectResult>(resultado);
        Assert.Equal(500, objectResult.StatusCode);

        var jsonString = System.Text.Json.JsonSerializer.Serialize(objectResult.Value);
        Assert.Contains("Erro interno inesperado ao buscar tarefas.", jsonString);
    }

    [Fact]
    public async Task GetId_DeveRetornarUnauthorized_QuandoUsuarioNaoAutenticado()
    {
        var userIdResult = Result.Fail<int>("Usuário não autenticado");
        var serviceMock = new Mock<ITaskService>();

        var controller = CriarControllerComUsuario(userIdResult, serviceMock);

        var resultado = await controller.GetId(1);

        var unauthorizedResult = Assert.IsType<UnauthorizedResult>(resultado);
        Assert.Equal(401, unauthorizedResult.StatusCode);
    }

    // Tests para EndPoint create  
    
    [Fact]
    public async Task Create_DeveRetornarCreated_QuandoSucesso()
    {
        var serviceMock = new Mock<ITaskService>();
        var createDto = new CreateTaskDto { Title = "Nova tarefa", Priority = "Media", Status = "EmAndamento"};
        var getDto = new GetTaskDto { Id = 1, Title = "Nova tarefa", Priority = "Media", Status = "EmAndamento" };

        serviceMock.Setup(s => s.CreateAsync(createDto, 1)).ReturnsAsync(Result.Ok(getDto));

        var controller = CriarControllerComUsuario(Result.Ok(1), serviceMock);

        var resultado = await controller.Create(createDto);

        var createdResult = Assert.IsType<CreatedAtActionResult>(resultado);
        Assert.Equal(201, createdResult.StatusCode);
    }

    [Fact]
    public async Task Create_DeveRetornarBadRequest_QuandoFalhaValidacao()
    {
        var serviceMock = new Mock<ITaskService>();
        var dto = new CreateTaskDto { Title = "",  Priority = "Media", Status = "EmAndamento" };

        serviceMock.Setup(s => s.CreateAsync(dto, 1)).ReturnsAsync(Result.Fail("Erro de validação"));

        var controller = CriarControllerComUsuario(Result.Ok(1), serviceMock);

        var resultado = await controller.Create(dto);

        var badRequest = Assert.IsType<BadRequestObjectResult>(resultado);
        Assert.Equal(400, badRequest.StatusCode);
    }

    [Fact]
    public async Task Create_DeveRetornarInternalServerError_QuandoErroInesperado()
    {
        var serviceMock = new Mock<ITaskService>();
        var createDto = new CreateTaskDto { Title = "Teste", Priority = "Media", Status = "EmAndamento" };

        serviceMock
            .Setup(s => s.CreateAsync(It.IsAny<CreateTaskDto>(), It.IsAny<int>()))
            .ReturnsAsync(Result.Fail<GetTaskDto>("Erro inesperado no banco de dados"));

        var controller = CriarControllerComUsuario(Result.Ok(1), serviceMock);

        var resultado = await controller.Create(createDto);

        var objectResult = Assert.IsType<ObjectResult>(resultado);
        Assert.Equal(500, objectResult.StatusCode);

        var jsonString = System.Text.Json.JsonSerializer.Serialize(objectResult.Value);
        Assert.Contains("Erro inesperado no banco de dados", jsonString);
    }

    [Fact]
    public async Task Create_DeveRetornarUnauthorized_QuandoUsuarioNaoAutenticado()
    {
        var userIdResult = Result.Fail<int>("Usuário não autenticado");
        var serviceMock = new Mock<ITaskService>();
        var createDto = new CreateTaskDto { Title = "Teste", Priority = "Media", Status = "EmAndamento" };

        var controller = CriarControllerComUsuario(userIdResult, serviceMock);

        var resultado = await controller.Create(createDto);

        var unauthorizedResult = Assert.IsType<UnauthorizedResult>(resultado);
        Assert.Equal(401, unauthorizedResult.StatusCode);
    }
    
    // Tests para EndPoint updata    
    
    [Fact]
    public async Task EditTask_DeveRetornarOk_QuandoAtualizacaoForBemSucedida()
    {
        var serviceMock = new Mock<ITaskService>();
        var dto = new CreateTaskDto { Title = "Atualizado", Priority = "Media", Status = "EmAndamento" };
        var retorno = new GetTaskDto { Id = 1, Title = "Atualizado",  Priority = "Media", Status = "EmAndamento" };

        serviceMock.Setup(s => s.UpdateAsync(dto, 1, 1)).ReturnsAsync(Result.Ok(retorno));

        var controller = CriarControllerComUsuario(Result.Ok(1), serviceMock);

        var resultado = await controller.EditTask(dto, 1);

        var okResult = Assert.IsType<OkObjectResult>(resultado);
        Assert.Equal(200, okResult.StatusCode);
    }

    [Fact]
    public async Task EditTask_DeveRetornarBadRequest_QuandoValidacaoFalhar()
    {
        var serviceMock = new Mock<ITaskService>();
        var dto = new CreateTaskDto { Title = "",  Priority = "Media", Status = "EmAndamento" };

        serviceMock.Setup(s => s.UpdateAsync(dto, 1, 1)).ReturnsAsync(Result.Fail("validacao falhou"));

        var controller = CriarControllerComUsuario(Result.Ok(1), serviceMock);

        var resultado = await controller.EditTask(dto, 1);

        var badRequest = Assert.IsType<BadRequestObjectResult>(resultado);
        Assert.Equal(400, badRequest.StatusCode);
    }
    
    [Fact]
    public async Task EditTask_DeveRetornarInternalServerError_QuandoErroInesperado()
    {
        var serviceMock = new Mock<ITaskService>();
        var editDto = new CreateTaskDto { Title = "Teste Atualizado", Priority = "Media", Status = "EmAndamento" };

        serviceMock
            .Setup(s => s.UpdateAsync(It.IsAny<CreateTaskDto>(), It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(Result.Fail<GetTaskDto>("Erro inesperado no banco de dados"));

        var controller = CriarControllerComUsuario(Result.Ok(1), serviceMock);

        var resultado = await controller.EditTask(editDto, 1);

        var objectResult = Assert.IsType<ObjectResult>(resultado);
        Assert.Equal(500, objectResult.StatusCode);

        var jsonString = System.Text.Json.JsonSerializer.Serialize(objectResult.Value);
        Assert.Contains("Erro inesperado no banco de dados", jsonString);
    }

    [Fact]
    public async Task EditTask_DeveRetornarUnauthorized_QuandoUsuarioNaoAutenticado()
    {
        var userIdResult = Result.Fail<int>("Usuário não autenticado");
        var serviceMock = new Mock<ITaskService>();
        var editDto = new CreateTaskDto { Title = "Teste Atualizado", Priority = "Media", Status = "EmAndamento" };

        var controller = CriarControllerComUsuario(userIdResult, serviceMock);

        var resultado = await controller.EditTask(editDto, 1);

        var unauthorizedResult = Assert.IsType<UnauthorizedResult>(resultado);
        Assert.Equal(401, unauthorizedResult.StatusCode);
    }

    
    // Testes para EndPoint updatestatus 
    
    [Fact]
    public async Task UpdateStatus_DeveRetornarOk_QuandoAtualizacaoForBemSucedida()
    {
        var serviceMock = new Mock<ITaskService>();
        var dto = new UpdateStatusTaskDto { Status = "Concluido" };

        serviceMock
            .Setup(s => s.UpadteStatusAsync(dto, 1, 1))
            .ReturnsAsync(Result.Ok());

        var controller = CriarControllerComUsuario(Result.Ok(1), serviceMock);

        var resultado = await controller.UpdateStatus(dto, 1);

        var okResult = Assert.IsType<OkObjectResult>(resultado);
        Assert.Equal(200, okResult.StatusCode);
    }

    [Fact]
    public async Task UpdateStatus_DeveRetornarUnauthorized_QuandoUsuarioNaoAutenticado()
    {
        var serviceMock = new Mock<ITaskService>();
        var dto = new UpdateStatusTaskDto { Status = "Concluido" };

        var controller = CriarControllerComUsuario(Result.Fail<int>("Usuário não autenticado"), serviceMock);

        var resultado = await controller.UpdateStatus(dto, 1);

        var unauthorizedResult = Assert.IsType<UnauthorizedResult>(resultado);
        Assert.Equal(401, unauthorizedResult.StatusCode);
    }

    [Fact]
    public async Task UpdateStatus_DeveRetornarBadRequest_QuandoFalhaDeValidacao()
    {
        var serviceMock = new Mock<ITaskService>();
        var dto = new UpdateStatusTaskDto { Status = "" };

        serviceMock
            .Setup(s => s.UpadteStatusAsync(dto, 1, 1))
            .ReturnsAsync(Result.Fail("validacao falhou"));

        var controller = CriarControllerComUsuario(Result.Ok(1), serviceMock);

        var resultado = await controller.UpdateStatus(dto, 1);

        var badRequest = Assert.IsType<BadRequestObjectResult>(resultado);
        Assert.Equal(400, badRequest.StatusCode);
    }

    [Fact]
    public async Task UpdateStatus_DeveRetornarInternalServerError_QuandoErroInesperado()
    {
        var serviceMock = new Mock<ITaskService>();
        var dto = new UpdateStatusTaskDto { Status = "EmAndamento" };

        serviceMock
            .Setup(s => s.UpadteStatusAsync(dto, 1, 1))
            .ReturnsAsync(Result.Fail("Erro inesperado no banco de dados"));

        var controller = CriarControllerComUsuario(Result.Ok(1), serviceMock);

        var resultado = await controller.UpdateStatus(dto, 1);

        var objectResult = Assert.IsType<ObjectResult>(resultado);
        Assert.Equal(500, objectResult.StatusCode);
        Assert.Equal("Erro inesperado no banco de dados", objectResult.Value);
    }
    
    // Testes para EndPoint updatepriority
    
    [Fact]
    public async Task UpdatePriority_DeveRetornarOk_QuandoAtualizacaoForBemSucedida()
    {
        var serviceMock = new Mock<ITaskService>();
        var dto = new UpdatePriorityDto { Priority = "Alta" };

        serviceMock
            .Setup(s => s.UpdatePriorityAsync(dto, 1, 1))
            .ReturnsAsync(Result.Ok());

        var controller = CriarControllerComUsuario(Result.Ok(1), serviceMock);

        var resultado = await controller.UpdatePriority(dto, 1);

        var okResult = Assert.IsType<OkObjectResult>(resultado);
        Assert.Equal(200, okResult.StatusCode);
    }

    [Fact]
    public async Task UpdatePriority_DeveRetornarUnauthorized_QuandoUsuarioNaoAutenticado()
    {
        var serviceMock = new Mock<ITaskService>();
        var dto = new UpdatePriorityDto { Priority = "Alta" };

        var controller = CriarControllerComUsuario(Result.Fail<int>("Usuário não autenticado"), serviceMock);

        var resultado = await controller.UpdatePriority(dto, 1);

        var unauthorizedResult = Assert.IsType<UnauthorizedResult>(resultado);
        Assert.Equal(401, unauthorizedResult.StatusCode);
    }

    [Fact]
    public async Task UpdatePriority_DeveRetornarBadRequest_QuandoFalhaDeValidacao()
    {
        var serviceMock = new Mock<ITaskService>();
        var dto = new UpdatePriorityDto { Priority = "" };

        serviceMock
            .Setup(s => s.UpdatePriorityAsync(dto, 1, 1))
            .ReturnsAsync(Result.Fail("validacao falhou"));

        var controller = CriarControllerComUsuario(Result.Ok(1), serviceMock);

        var resultado = await controller.UpdatePriority(dto, 1);

        var badRequest = Assert.IsType<BadRequestObjectResult>(resultado);
        Assert.Equal(400, badRequest.StatusCode);
    }

    [Fact]
    public async Task UpdatePriority_DeveRetornarInternalServerError_QuandoErroInesperado()
    {
        var serviceMock = new Mock<ITaskService>();
        var dto = new UpdatePriorityDto { Priority = "Baixa" };

        serviceMock
            .Setup(s => s.UpdatePriorityAsync(dto, 1, 1))
            .ReturnsAsync(Result.Fail("Erro inesperado no banco de dados"));

        var controller = CriarControllerComUsuario(Result.Ok(1), serviceMock);

        var resultado = await controller.UpdatePriority(dto, 1);

        var objectResult = Assert.IsType<ObjectResult>(resultado);
        Assert.Equal(500, objectResult.StatusCode);
        Assert.Equal("Erro inesperado no banco de dados", objectResult.Value);
    }

    // Testes para EndPoint delete
    
    [Fact]
    public async Task Delete_DeveRetornarNoContent_QuandoDelecaoForBemSucedida()
    {
        var serviceMock = new Mock<ITaskService>();
        serviceMock
            .Setup(s => s.DeleteAsync(1, 1))
            .ReturnsAsync(Result.Ok());

        var controller = CriarControllerComUsuario(Result.Ok(1), serviceMock);

        var resultado = await controller.Delete(1);

        var noContentResult = Assert.IsType<NoContentResult>(resultado);
        Assert.Equal(204, noContentResult.StatusCode);
    }

    [Fact]
    public async Task Delete_DeveRetornarUnauthorized_QuandoUsuarioNaoAutenticado()
    {
        var serviceMock = new Mock<ITaskService>();

        var controller = CriarControllerComUsuario(Result.Fail<int>("Usuário não autenticado"), serviceMock);

        var resultado = await controller.Delete(1);

        var unauthorizedResult = Assert.IsType<UnauthorizedResult>(resultado);
        Assert.Equal(401, unauthorizedResult.StatusCode);
    }

    [Fact]
    public async Task Delete_DeveRetornarInternalServerError_QuandoErroInesperado()
    {
        // Arrange
        var serviceMock = new Mock<ITaskService>();
        serviceMock
            .Setup(s => s.DeleteAsync(1, 1))
            .ReturnsAsync(Result.Fail("Erro inesperado no banco de dados"));

        var controller = CriarControllerComUsuario(Result.Ok(1), serviceMock);

        // Act
        var resultado = await controller.Delete(1);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(resultado);
        Assert.Equal(500, objectResult.StatusCode);

        // Aqui acessamos as mensagens diretamente
        var mensagens = ((IEnumerable<IError>)objectResult.Value)
            .Select(e => e.Message)
            .ToList();

        Assert.Contains("Erro inesperado no banco de dados", mensagens);
    }
}
