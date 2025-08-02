using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Interfaces.ITask;

namespace TaskManager.Infrastructure.Data.Repositories.TaskR;

public class TaskRepository : ITaskRepository
{
    private readonly TaskManagerContext _context;
    private readonly ILogger<TaskRepository> _logger;

    public TaskRepository(TaskManagerContext context, ILogger<TaskRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<TaskE>> GetByUserIdAsync(int userId)
    {
        _logger.LogInformation("Buscando tarefas para o usuário com ID {UserId}", userId);

        return await _context.Tarefas
            .AsNoTracking()
            .Where(t => t.UsuarioId == userId)
            .ToListAsync();
    }

    public async Task<TaskE?> GetId(int id, int userId)
    {
        _logger.LogInformation("Buscando tarefas com id: {Id}", id);

        return await _context.Tarefas
            .FirstOrDefaultAsync(t => t.Id == id && t.UsuarioId == userId);
    }

    public async Task<TaskE> CreateTask(TaskE task)
    {
        _logger.LogInformation("Creando nova tarefa com nome: {Nome}", task.Title);

        _context.Tarefas.Add(task);
        await _context.SaveChangesAsync();
        return task;

    }

    public async Task EditTask(TaskE task)
    {
        _logger.LogInformation("Editando a tarefa de id: {Id}", task.Id);
        
        _context.Tarefas.Update(task);
        await _context.SaveChangesAsync();
    }
}
