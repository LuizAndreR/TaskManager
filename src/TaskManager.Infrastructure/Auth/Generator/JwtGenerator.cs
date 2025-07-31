using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace TaskManager.Infrastructure.Auth.Generator;

public class JwtGenerator : IJwtGenerator
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<JwtGenerator> _logger;

    public JwtGenerator(IConfiguration configuration, ILogger<JwtGenerator> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public string Generate(string email, int id)
    {
        var key = Encoding.ASCII.GetBytes(_configuration.GetSection("Jwt")["Key"]!);

        var tokenDescript = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, id.ToString())
            }),
            Expires = DateTime.UtcNow.AddHours(24),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature
            )
        };

        var tokenHendler = new JwtSecurityTokenHandler();
        var token = tokenHendler.CreateToken(tokenDescript);

        _logger.LogInformation("Gerando token para o email: {Email}", email);

        return tokenHendler.WriteToken(token);
    }
}
