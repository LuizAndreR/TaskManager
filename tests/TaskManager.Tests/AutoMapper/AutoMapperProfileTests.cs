using AutoMapper;
using TaskManager.Application.Dtos.Auth;
using TaskManager.Application.Dtos.TaskDto;
using TaskManager.Application.Profiles;
using TaskManager.Domain.Entities;

namespace TaskManager.Tests.AutoMapper;

public class AutoMapperProfileTests
{
    private readonly IMapper _mapper;

    public AutoMapperProfileTests()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<CadastroProfile>();
            cfg.AddProfile<TaskProfile>();
        });

        _mapper = config.CreateMapper();

        config.AssertConfigurationIsValid();
    }

    [Fact]
    public void Deve_Mapear_CriarUsuarioRequest_Para_Usuario()
    {
        var dto = new CadastroUsuarioRequest
        {
            Nome = "Teste",
            Email = "teste@email.com",
            Senha = "Senha123!"
        };

        var usuario = _mapper.Map<Usuario>(dto);

        Assert.Equal(dto.Nome, usuario.Nome);
        Assert.Equal(dto.Email, usuario.Email);
    }

    [Fact]
    public void Deve_Mapear_TaskE_Para_GetTaskDto()
    {
        var usuario = new Usuario
        {
            Nome = "Teste",
            Email = "teste@gmail.com",
            SenhaHash = "Senha123!"
        };

        var task = new TaskE
        {
            Title = "Teste de Tarefa",
            Descriptions = "Descrição da tarefa",
            Priority = "Alta",
            Status = "Pendente",
            DateCreated = DateTime.UtcNow,
            UsuarioId = 1,
            Usuario = usuario
        };

        var taskDto = _mapper.Map<GetTaskDto>(task);

        Assert.Equal(task.Title, taskDto.Title);
        Assert.Equal(task.Descriptions, taskDto.Descriptions);
    }

    [Fact]
    public void Deve_Mapear_Criartask_Para_TaskE()
    {
        var dto = new CreateTaskDto
        {
            Title = "Nova Tarefa",
            Descriptions = "Descrição da nova tarefa",
            Priority = "Média",
            Status = "Pendente"
        };

        var task = _mapper.Map<TaskE>(dto);

        Assert.Equal(dto.Title, task.Title);
        Assert.Equal(dto.Descriptions, task.Descriptions);
    }

}
