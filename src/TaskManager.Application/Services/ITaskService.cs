using FluentResults;
using TaskManager.Application.Dtos.TaskDto;

namespace TaskManager.Application.Services;

public interface ITaskService
{
    public Task<Result<IEnumerable<GetTaskDto>>> GetAllAsync(int userId);
    public Task<Result<GetTaskDto>> GetByIdAsync(int id, int userId);
    public Task<Result<GetTaskDto>> CreateAsync(CreateTaskDto request, int userId);
    public Task<Result<GetTaskDto>> UpdateAsync(CreateTaskDto request, int idTask, int userId);
    public Task<Result> UpadteStatusAsync(UpdateStatusTaskDto request, int idTask, int userId);
    public Task<Result> UpdatePriorityAsync(UpdatePriorityDto request, int idTask, int userId);
    public Task<Result> DeleteAsync(int id, int userId);
}