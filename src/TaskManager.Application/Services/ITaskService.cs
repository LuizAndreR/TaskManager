using FluentResults;
using TaskManager.Application.Dtos.TaskDto;

namespace TaskManager.Application.Services;

public interface ITaskService
{
    public Task<Result<IEnumerable<GetTaskDto>>> GetAllAsync(int userId);
    public Task<Result<GetTaskDto>> GetByIdAsync(int id, int userId);
    public Task<Result<GetTaskDto>> CreateAsync(CreateTaskDto createDto, int userId);
    public Task<Result<GetTaskDto>> UpdateAsync(CreateTaskDto editDto, int idTask, int userId);
}