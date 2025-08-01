using AutoMapper;
using TaskManager.Application.Dtos.Auth;
using TaskManager.Application.Dtos.TaskDto;
using TaskManager.Domain.Entities;

namespace TaskManager.Tests.Utils;

public class AutoMapperTestProfile : Profile
{
    public AutoMapperTestProfile()
    {
        CreateMap<CadastroUsuarioRequest, Usuario>()
            .ForMember(dest => dest.Tarefas, opt => opt.Ignore());
    }
}
