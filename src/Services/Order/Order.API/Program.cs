using Carter;
using MessageBroker.MassTransit;
using Microsoft.EntityFrameworkCore;
using Order.API.Data;
using Price.Grpc;
using Serilog;
using Shared.Exceptions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var assembly = typeof(Program).Assembly;
builder.Services.AddMediatR(config => config.RegisterServicesFromAssembly(assembly));

builder.Services.AddMessageBroker(builder.Configuration, assembly);
builder.Services.AddCarter();

builder.Services.AddDbContext<OrderDbContext>(options =>
               options.UseNpgsql(builder.Configuration.GetConnectionString("Database")));

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddGrpcClient<StockPriceProtoService.StockPriceProtoServiceClient>(options =>
{
    options.Address = new Uri(builder.Configuration["GrpcSettings:StockPriceApiUrl"] ?? throw new Exception("Stock price api url not configured"));
})
//Fix ssl sertificate don't user it on prod
.ConfigurePrimaryHttpMessageHandler(() =>
{
    var handler = new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback =
        HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
    };

    return handler;
});

builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.MapCarter();

app.Run();