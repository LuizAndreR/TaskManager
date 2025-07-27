using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Serilog;
using TaskManager.Application.UseCase.Auth.Cadastro;
using TaskManager.Application.UseCase.Auth.Login;
using TaskManager.Application.Validator.Auth;
using TaskManager.Domain.Interfaces.Auth;
using TaskManager.Infrastructure.Auth.Generator;
using TaskManager.Infrastructure.Data.Context;
using TaskManager.Infrastructure.Data.Repositories.Auth;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Services.AddDbContext<TaskManagerContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection"),
        x => x.MigrationsAssembly("TaskManager.Infrastructure")));

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Host.UseSerilog();

builder.Services.AddControllers();
builder.Services.AddScoped<ICadastroUseCase, CadastroUseCase>();
builder.Services.AddScoped<ILoginUseCase, LoginUseCase>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IJwtGenerator, JwtGenerator>();
builder.Services.AddValidatorsFromAssemblyContaining<CadastroValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<LoginValidator>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();       
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
try
{
    Log.Information("Iniciando aplicação ReservaDezoito...");
    app.Run();
}
catch(Exception ex)
{
    Log.Fatal(ex, "A aplicação falhou ao iniciar");
}
finally
{
    Log.CloseAndFlush();
}

