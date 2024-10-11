using Carter;
using MessageBroker.MassTransit;
using Microsoft.EntityFrameworkCore;
using Portfolio.API.Data;
using Serilog;
using Shared.Exceptions;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var assembly = typeof(Program).Assembly;
builder.Services.AddMediatR(config => config.RegisterServicesFromAssembly(assembly));

builder.Services.AddMessageBroker(builder.Configuration, assembly);
builder.Services.AddCarter();

builder.Services.AddDbContext<PortfolioDbContext>(options =>
               options.UseNpgsql(builder.Configuration.GetConnectionString("Database")));

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    using (var scope = app.Services.CreateScope())
    {
        var contex = scope.ServiceProvider.GetRequiredService<PortfolioDbContext>();
        contex.Database.Migrate();
    }
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.MapCarter();

app.UseExceptionHandler(options => { });

app.Run();
