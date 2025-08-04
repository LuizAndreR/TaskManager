using TaskManager.Domain.Entities;

namespace TaskManager.Domain.Interfaces.ITask;

public interface ITaskRepository
{
    public Task<IEnumerable<TaskE>> GetByUserIdAsync(int id);
    public Task<TaskE?> GetId(int id, int userId);
    public Task<TaskE> CreateTask(TaskE task);
    public Task EditTask(TaskE task);
    public Task DeleteTask(TaskE task, int userId);
}
