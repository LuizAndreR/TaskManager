using AutoMapper;
using TaskManager.Application.Dtos.TaskDto;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Profiles;

public class TaskProfile : Profile
{
    public TaskProfile()
    {
        CreateMap<GetTaskDto, TaskE>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.UsuarioId, opt => opt.Ignore())
            .ForMember(dest => dest.Usuario, opt => opt.Ignore())
            .ReverseMap();

        CreateMap<CreateTaskDto, TaskE>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.UsuarioId, opt => opt.Ignore())
            .ForMember(dest => dest.Usuario, opt => opt.Ignore())
            .ForMember(dest => dest.DateCreated, opt => opt.Ignore())
            .ReverseMap();
    }
}
