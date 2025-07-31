using AutoMapper;
using TaskManager.Application.Dtos.TaskDto;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Profiles;

public class TaskProfile : Profile
{
    public TaskProfile()
    {
        CreateMap<GetTaskDto, TaskE>()
            .ReverseMap();

        CreateMap<CreateTaskDto, TaskE>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ReverseMap();
    }
}
