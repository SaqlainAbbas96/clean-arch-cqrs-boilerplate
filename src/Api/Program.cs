using Api.Endpoints;
using Api.Infrastructure;
using Application.Commands;
using FluentValidation;
using Infrastructure.Data;
using Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Framework Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddValidatorsFromAssemblyContaining<CreateTaskValidator>();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// Application Services
builder.Services.AddScoped<CreateTaskHandler>();
builder.Services.AddScoped<GetAllTasksHandler>();
builder.Services.AddScoped<CreateTaskValidator>();

// Infrastructure Services
builder.Services.AddInfrastructureServices();
builder.Services.AddDatabase(builder.Configuration);

var app = builder.Build();

if (!app.Environment.IsEnvironment("Testing"))
{
    using (var scope = app.Services.CreateScope())
    {
        var dbInit = scope.ServiceProvider.GetRequiredService<DbInitializer>();
        dbInit.Initialize();
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseExceptionHandler();
app.MapTaskEndpoints();

app.Run();
public partial class Program { }