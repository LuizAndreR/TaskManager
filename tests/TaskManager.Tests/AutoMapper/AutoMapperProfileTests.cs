using AutoMapper;
using TaskManager.Application.Dtos.Auth;
using TaskManager.Application.Profiles;
using TaskManager.Domain.Entities;

namespace ReservaDezoito.Tests.AutoMapper;

public class AutoMapperProfileTests
{
    private readonly IMapper _mapper;

    public AutoMapperProfileTests()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<CadastroProfile>();
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
}
