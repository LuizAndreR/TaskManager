using FluentResults;
using Moq;
using TaskManager.Application.Dtos.TaskDto;
using TaskManager.Application.Services;
using TaskManager.Application.UseCase.Tasks.Interfaces;

namespace TaskManager.Tests.Service;

public class TaskServiceTests
{
    private readonly Mock<ICreateTaskUseCase> _createTaskMock;
    private readonly Mock<IGetAllTasksUseCase> _getAllTasksMock;
    private readonly Mock<IGetTaskByIdUseCase> _getTaskByIdMock;
    private readonly Mock<IUpdateTaskUseCase> _updateTaskMock;
    private readonly TaskService _taskService;

    public TaskServiceTests()
    {
        _createTaskMock = new Mock<ICreateTaskUseCase>();
        _getAllTasksMock = new Mock<IGetAllTasksUseCase>();
        _getTaskByIdMock = new Mock<IGetTaskByIdUseCase>();
        _updateTaskMock = new Mock<IUpdateTaskUseCase>();

        _taskService = new TaskService(
            _createTaskMock.Object,
            _getAllTasksMock.Object,
            _getTaskByIdMock.Object,
            _updateTaskMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnTasks_WhenSuccessful()
    {
        var tarefas = new List<GetTaskDto> { new() { Id = 1, Title = "Teste", Priority = "Media", Status = "EmAndamento"} };
        _getAllTasksMock.Setup(x => x.GetAllTasksAsync(1))
            .ReturnsAsync(Result.Ok(tarefas.AsEnumerable()));

        var result = await _taskService.GetAllAsync(1);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnTask_WhenFound()
    {
        var tarefa = new GetTaskDto { Id = 1, Title = "Teste", Priority = "Media", Status = "EmAndamento"};
        _getTaskByIdMock.Setup(x => x.GetTaskByIdAsync(1, 1))
            .ReturnsAsync(Result.Ok(tarefa));

        var result = await _taskService.GetByIdAsync(1, 1);

        Assert.True(result.IsSuccess);
        Assert.Equal(1, result.Value.Id);
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnCreatedTask_WhenSuccessful()
    {
        var dto = new CreateTaskDto { Title = "Nova Tarefa",  Priority = "Media", Status = "EmAndamento" };
        var created = new GetTaskDto { Id = 2, Title = "Nova Tarefa",  Priority = "Media", Status = "EmAndamento" };

        _createTaskMock.Setup(x => x.CreateTaskAsync(dto, 1))
            .ReturnsAsync(Result.Ok(created));

        var result = await _taskService.CreateAsync(dto, 1);

        Assert.True(result.IsSuccess);
        Assert.Equal("Nova Tarefa", result.Value.Title);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnUpdatedTask_WhenSuccessful()
    {
        var dto = new CreateTaskDto { Title = "Atualizada",  Priority = "Media", Status = "EmAndamento" };
        var updated = new GetTaskDto { Id = 1, Title = "Atualizada",  Priority = "Media", Status = "EmAndamento" };

        _updateTaskMock.Setup(x => x.UpdateTaskAsync(dto, 1, 1))
            .ReturnsAsync(Result.Ok(updated));

        var result = await _taskService.UpdateAsync(dto, 1, 1);

        Assert.True(result.IsSuccess);
        Assert.Equal("Atualizada", result.Value.Title);
    }
} 
