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
    private readonly IUpdateStatusTaskUseCase _updateStatusTask;
    private readonly IUpdatePriorityUseCase _updatePriority;
    private readonly IDeleteTaskUseCase _deleteTask;

    public TaskService(
        ICreateTaskUseCase createTask,
        IGetAllTasksUseCase getAllTasks,
        IGetTaskByIdUseCase getTaskById,
        IUpdateTaskUseCase updateTask,
        IUpdateStatusTaskUseCase updateStatusTask,
        IUpdatePriorityUseCase updatePriority,
        IDeleteTaskUseCase deleteTask)
    {
        _createTask = createTask;
        _getAllTasks = getAllTasks;
        _getTaskById = getTaskById;
        _updateTask = updateTask;
        _updateStatusTask = updateStatusTask;
        _updatePriority = updatePriority;
        _deleteTask = deleteTask;
    }
    
    public async Task<Result<IEnumerable<GetTaskDto>>> GetAllAsync(int userId) => await _getAllTasks.GetAllTasksAsync(userId);

    public async Task<Result<GetTaskDto>> GetByIdAsync(int id, int userId) => await _getTaskById.GetTaskByIdAsync(id, userId);

    public async Task<Result<GetTaskDto>> CreateAsync(CreateTaskDto request, int userId) => await _createTask.CreateTaskAsync(request, userId);

    public async Task<Result<GetTaskDto>> UpdateAsync(CreateTaskDto request, int idTask, int userId) => await _updateTask.UpdateTaskAsync(request, idTask, userId);

    public async Task<Result> UpadteStatusAsync(UpdateStatusTaskDto request, int idTask, int userId) => await _updateStatusTask.UpdateStatusAsync(request, idTask, userId);

    public async Task<Result> UpdatePriorityAsync(UpdatePriorityDto request, int idTask, int userId) => await _updatePriority.UpdatePriorityAsync(request, idTask, userId);

    public async Task<Result> DeleteAsync(int id, int userId) => await _deleteTask.DeleteTaskAsync(id, userId);
}