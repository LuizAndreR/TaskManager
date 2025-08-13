using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;
using TaskManager.Application.Services;
using TaskManager.Application.UseCase.Auth.Cadastro;
using TaskManager.Application.UseCase.Auth.Login;
using TaskManager.Application.UseCase.Tasks.Interfaces;
using TaskManager.Application.UseCase.Tasks.UseCases;
using TaskManager.Application.Validator.Auth;
using TaskManager.Application.Validator.TaskV;
using TaskManager.Domain.Interfaces.Auth;
using TaskManager.Domain.Interfaces.ITask;
using TaskManager.Infrastructure.Auth.Generator;
using TaskManager.Infrastructure.Data;
using TaskManager.Infrastructure.Data.Repositories.Auth;
using TaskManager.Infrastructure.Data.Repositories.TaskR;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var key = builder.Configuration["Jwt:Key"];

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(key!))
        };
    });

builder.Services.AddDbContext<TaskManagerContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection"),
        x => x.MigrationsAssembly("TaskManager.Infrastructure")));

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Host.UseSerilog();

builder.Services.AddControllers();
builder.Services.AddScoped<ITaskService, TaskService>();

// UseCases Tasks
builder.Services.AddScoped<ICreateTaskUseCase, CreateTaskUseCase>();
builder.Services.AddScoped<IGetAllTasksUseCase, GetAllTasksUseCase>();
builder.Services.AddScoped<IGetTaskByIdUseCase, GetTaskByIdUseCase>();
builder.Services.AddScoped<IUpdateTaskUseCase, UpdateTaskUseCase>();
builder.Services.AddScoped<IUpdateStatusTaskUseCase, UpdateStatusTaskUseCase>();
builder.Services.AddScoped<IUpdatePriorityUseCase, UpdatePriorityUseCase>();
builder.Services.AddScoped<IDeleteTaskUseCase, DeleteTaskUseCase>();

// UseCases Auth
builder.Services.AddScoped<ICadastroUseCase, CadastroUseCase>();
builder.Services.AddScoped<ILoginUseCase, LoginUseCase>();

// Repositories
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<IJwtGenerator, JwtGenerator>();

// Validators
builder.Services.AddValidatorsFromAssemblyContaining<CadastroValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<LoginValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UpdateStatusValidator>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();       
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
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

