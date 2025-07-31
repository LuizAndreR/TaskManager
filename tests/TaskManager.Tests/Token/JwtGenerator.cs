using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using TaskManager.Infrastructure.Auth.Generator;

namespace TaskManager.Tests.Token;

public class JwtGeneratorTest
{
    private Mock<ILogger<JwtGenerator>> _logger;

    public JwtGeneratorTest()
    {
        _logger = new Mock<ILogger<JwtGenerator>>();
    }

    [Fact]
    public void GerarToken_DeveRetornarTokenValido_ComClaimDeEmail()
    {
        var email = "teste@email.com";
        var id = 1;
        var jwtKey = "jA93!@sm29FK_49sDlslaQWopqndas!$wq4Aa2B8@z!XkM3$uVp9R#qLwF7vHs1YcEzT";

        var mockSection = new Mock<IConfigurationSection>();
        mockSection.Setup(s => s["Key"]).Returns(jwtKey);

        var mockConfig = new Mock<IConfiguration>();
        mockConfig.Setup(c => c.GetSection("Jwt")).Returns(mockSection.Object);

        var tokenService = new JwtGenerator(mockConfig.Object, _logger.Object);

        var token = tokenService.Generate(email, id);

        Assert.False(string.IsNullOrWhiteSpace(token));
    }
}
