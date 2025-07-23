using BloggerAgent.Domain.Data;
using BloggerAgent.Domain.IRepositories;
using BloggerAgent.Application.IServices;
using BloggerAgent.Api.Middleware;
using BloggerAgent.Infrastructure.Repositories;
using BloggerAgent.Infrastructure.Services;
using BloggerAgent.Domain.DomainHelper;
using BloggerAgent.Infrastructure.Tooling;
using BloggerAgent.Domain.Repositories;
using BloggerAgent.Infrastructure.Tooling.Context;
using BloggerAgent.Infrastructure.Tooling.Types;
using Serilog;
using Serilog.Events;
using OpenTelemetry.Trace;
using BloggerAgent.Application.Configurations;
using VigilAgent.Apm.Middleware;
using BloggerAgent.Infrastructure.ToolFunctions;
using BloggerAgent.Infrastructure.Commons;
using BloggerAgent.Domain.Commons.Options;
using BloggerAgent.Infrastructure.Commons.BloggerAgent.Infrastructure.Commons;
using Microsoft.Extensions.Caching.Memory;

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
// Bind MongoDB settings

builder.Services.Configure<TelexApiSettings>(builder.Configuration.GetSection("MongoDbConfig"));

builder.Services.Configure<GeminiSetting>(builder.Configuration.GetSection("GeminiSetting"));
builder.Services.Configure<TelexSetting>(builder.Configuration.GetSection("TelexSetting"));

builder.Services.Configure<TelexApiSettings>(builder.Configuration.GetSection("TelexApiSettings"));
builder.Services.AddScoped<DbContext>();
builder.Services.AddScoped<HttpHelper>();
builder.Services.AddTelemetryExporter(builder.Configuration) ;

Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT", "http://localhost:4318");
Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_PROTOCOL", "http/protobuf");
Environment.SetEnvironmentVariable("OTEL_SERVICE_NAME", "blogger-agent");

//builder.Services.AddOpenTelemetry().WithTracing(tracerProviderBuilder =>
//{
//    tracerProviderBuilder
//        .AddAspNetCoreInstrumentation()
//        .AddHttpClientInstrumentation()
//        .AddSource("Microsoft.SemanticKernel*")
//        .AddConsoleExporter() // You can add other exporters here
//        .AddOtlpExporter();
//});

builder.Services.AddSingleton<BlogPlugin>();
builder.Services.AddSingleton<BlogAgentFunctions>();
builder.Services.AddSingleton<OrganizationPlugin>();
builder.Services.AddSingleton<AgentPlugin>();
builder.Services.AddSingleton<KernelProvider>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddHttpClient();
builder.Services.AddScoped<IBlogAgentService, BlogAgentService>();
builder.Services.AddScoped<IResearchAgent, ResearchAgent>();
builder.Services.AddScoped<OutlineAgent>();
builder.Services.AddScoped<WriterAgent>();
builder.Services.AddScoped<IBlogPostIntervalService, BlogPostIntervalService>(); 

builder.Services.AddScoped(typeof(ITelexRepository<>), typeof(TelexRepository<>));
builder.Services.AddScoped<IConversationRepository, ConversationRepository>();
builder.Services.AddScoped<IBlogRepository, BlogRepository>();
builder.Services.AddScoped<IRequestProcessingService, RequestProcessor>();
builder.Services.AddScoped<IAIService, AIService>();
builder.Services.AddScoped<ILlmTool, SaveOrganizationContextTool>();
builder.Services.AddScoped<ILlmTool, GetOrganizationContextTool>();
builder.Services.AddScoped<IOrganizationRepository, OrganizationRepository>();
builder.Services.AddScoped<ToolRouter>();
builder.Services.AddScoped<TaskContextAccessor>();
builder.Services.AddScoped<IApiKeyRepository, ApiKeyRepository>();
builder.Services.AddScoped<IMemoryCache, MemoryCache>();
builder.Services.AddScoped<OrgApiKeyStore>();
builder.Services.AddScoped<TaskManager>();

builder.Services.AddMemoryCache();

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
app.UseVigilTelemetry();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
