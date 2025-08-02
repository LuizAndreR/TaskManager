using AutoMapper;
using FluentResults;
using FluentValidation;
using Microsoft.Extensions.Logging;
using TaskManager.Application.Dtos.TaskDto;
using TaskManager.Application.UseCase.Tasks.Interfaces;
using TaskManager.Domain.Interfaces.ITask;

namespace TaskManager.Application.UseCase.Tasks.UseCases;

public class UpdateStatusTaskUseCase : IUpdateStatusTaskUseCase
{
    private readonly ITaskRepository _repository;
    private readonly IMapper _mapper;
    private readonly IValidator<UpdateStatusTaskDto> _validator;
    private readonly ILogger<UpdateStatusTaskUseCase> _logger;

    public UpdateStatusTaskUseCase(ITaskRepository repository, IMapper mapper,
        IValidator<UpdateStatusTaskDto> validator, ILogger<UpdateStatusTaskUseCase> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _validator = validator;
        _logger = logger;
    }
    
    public Task<Result> UpdateStatus(UpdateStatusTaskDto request, int id)
    {
        var result = _validator.Validate(request);

        if (!result.IsValid)
        {
            
        }
    }
}