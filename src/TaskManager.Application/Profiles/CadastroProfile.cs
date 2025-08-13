using AutoMapper;
using TaskManager.Application.Dtos.Auth;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Profiles;

public class CadastroProfile : Profile
{
    public CadastroProfile()
    {
        CreateMap<CadastroUsuarioRequest, Usuario>()
            .ForMember(dest => dest.SenhaHash, opt => opt.Ignore())
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Tarefas, opt => opt.Ignore());
    }
}
