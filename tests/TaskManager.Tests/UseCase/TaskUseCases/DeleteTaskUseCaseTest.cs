using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TaskManager.Application.Dtos.TaskDto;
using TaskManager.Application.UseCase.Tasks.UseCases;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Interfaces.ITask;

namespace TaskManager.Tests.UseCase.TaskUseCases;

public class DeleteTaskUseCaseTest
{
    private readonly Mock<ITaskRepository> _repoMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly DeleteTaskUseCase _useCase;

    public DeleteTaskUseCaseTest()
    {
        _repoMock = new Mock<ITaskRepository>();
        _mapperMock = new Mock<IMapper>();
        var loggerMock = new Mock<ILogger<DeleteTaskUseCase>>();

        _useCase = new DeleteTaskUseCase(_repoMock.Object, _mapperMock.Object, loggerMock.Object);
    }
    
    [Fact]
    public async Task DeleteAsync_DeveRetornarOK()
    {
        var usuario = new Usuario { Id = 5, Nome = "Teste", Email = "teste@gmail.com", SenhaHash = "Teste" };
        var tarefa = new TaskE { Id = 1, Title = "Teste", Status = "EmAndamento", Priority = "Media", Usuario = usuario};

        _repoMock
            .Setup(r => r.GetId(tarefa.Id, usuario.Id))
            .ReturnsAsync(tarefa);

        _repoMock
            .Setup(r => r.DeleteTask(tarefa, usuario.Id))
            .Returns(Task.CompletedTask);

        
        var result = await _useCase.DeleteTaskAsync(tarefa.Id, usuario.Id);
        
        Assert.True(result.IsSuccess);
        _repoMock.Verify(r => r.GetId(tarefa.Id, usuario.Id), Times.Once);
        _repoMock.Verify(r => r.DeleteTask(tarefa, usuario.Id), Times.Once);
    }
    
    [Fact]
    public async Task DeleteTaskAsync_DeveRetornarErro_QuandoExcecaoForLancada()
    {
        var usuario = new Usuario { Id = 5, Nome = "Teste", Email = "teste@gmail.com", SenhaHash = "Teste" };
        var taskId = 1;

        _repoMock
            .Setup(r => r.GetId(taskId, usuario.Id))
            .ThrowsAsync(new Exception("Falha no banco"));

        var result = await _useCase.DeleteTaskAsync(taskId, usuario.Id);
        
        Assert.True(result.IsFailed); // Agora deve ser true
        Assert.Contains("Erro inesperado ao deletar tarefa", result.Errors[0].Message);
    }

}