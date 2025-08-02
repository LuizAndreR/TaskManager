using FluentResults;
using TaskManager.Application.Dtos.TaskDto;
using TaskManager.Application.UseCase.Tasks.Interfaces;

namespace TaskManager.Application.Services;

public class TaskService : ITaskService
{
    
    private readonly ICreateTaskUseCase _createTask;
    private readonly IGetAllTasksUseCase _getAllTasks;
    private readonly IGetTaskByIdUseCase _getTaskById;
    private readonly IUpdateTaskUseCase _updateTask;

    public TaskService(
        ICreateTaskUseCase createTask,
        IGetAllTasksUseCase getAllTasks,
        IGetTaskByIdUseCase getTaskById,
        IUpdateTaskUseCase updateTask)
    {
        _createTask = createTask;
        _getAllTasks = getAllTasks;
        _getTaskById = getTaskById;
        _updateTask = updateTask;
    }
    
    public async Task<Result<IEnumerable<GetTaskDto>>> GetAllAsync(int userId) => await _getAllTasks.GetAllTasksAsync(userId);

    public async Task<Result<GetTaskDto>> GetByIdAsync(int id, int userId) => await _getTaskById.GetTaskByIdAsync(id, userId);

    public async Task<Result<GetTaskDto>> CreateAsync(CreateTaskDto createDto, int userId) => await _createTask.CreateTaskAsync(createDto, userId);

    public async Task<Result<GetTaskDto>> UpdateAsync(CreateTaskDto editDto, int idTask, int userId) => await _updateTask.UpdateTaskAsync(editDto, idTask, userId);
}