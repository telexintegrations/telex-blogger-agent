using BloggerAgent.Domain.Data;
using BloggerAgent.Api.Middleware;
using BloggerAgent.Domain.DomainHelper;
using Serilog;
using BloggerAgent.Application.Configurations;
using VigilAgent.Apm.Middleware;
using BloggerAgent.Domain.Commons.Options;
using Microsoft.Extensions.Caching.Memory;
using BloggerAgent.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Host.UseSerilog((context, services, options) =>
{
    options.Enrich.FromLogContext()
    .MinimumLevel.Information()
    .WriteTo.Console(outputTemplate:
        "{Level:u3}[{Timestamp:HH:mm:ss}] {Message:lj}{NewLine}");
});

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<TelexApiSettings>(builder.Configuration.GetSection("MongoDbConfig"));
builder.Services.Configure<GeminiSetting>(builder.Configuration.GetSection("GeminiSetting"));
builder.Services.Configure<TelexSetting>(builder.Configuration.GetSection("TelexSetting"));
builder.Services.Configure<TelexApiSettings>(builder.Configuration.GetSection("TelexApiSettings"));

builder.Services.AddTelemetryExporter(builder.Configuration);
builder.Services.AddScoped<IMemoryCache, MemoryCache>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddMemoryCache();
builder.Services.AddDIServices();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAnyOrigin", policy =>
    {
        policy.AllowAnyOrigin()
               .AllowAnyHeader()
               .AllowAnyMethod();
    });
});


var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var kernelProvider = scope.ServiceProvider.GetRequiredService<KernelProvider>();
    kernelProvider.RegisterPlugins(scope.ServiceProvider);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAnyOrigin");

app.UseMiddleware<ExceptionHandler>();

app.UseMiddleware<RequestLoggingMiddleware>();

//app.UseVigilTelemetry();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
