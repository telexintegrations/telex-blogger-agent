using BloggerAgent.Domain.Data;
using BloggerAgent.Domain.IRepositories;
using BloggerAgent.Application.IServices;
using BloggerAgent.Api.Middleware;
using BloggerAgent.Infrastructure.Repositories;
using BloggerAgent.Services;
using BloggerAgent.Infrastructure.Services;
using BloggerAgent.Domain.Commons;
using BloggerAgent.Domain.DomainService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Bind MongoDB settings

builder.Services.Configure<DatabaseConfig>(builder.Configuration.GetSection("MongoDbConfig"));

builder.Services.Configure<GeminiSetting>(builder.Configuration.GetSection("GeminiSetting"));
builder.Services.Configure<TelexSetting>(builder.Configuration.GetSection("TelexSetting"));

builder.Services.Configure<DatabaseConfig>(builder.Configuration.GetSection("MongoDbSettings"));
builder.Services.AddScoped<DbContext>();
builder.Services.AddScoped<HttpHelper>();


builder.Services.AddHttpClient();
builder.Services.AddTransient<IBlogAgentService, BlogAgentService>();
builder.Services.AddTransient<IBlogPostIntervalService, BlogPostIntervalService>(); 

builder.Services.AddScoped(typeof(IMongoRepository<>), typeof(MongoRepository<>));
builder.Services.AddScoped<ITelexIntegrationService, TelexIntegrationService>();
builder.Services.AddScoped<IConversationRepository, ConversationRepository>();
builder.Services.AddScoped<IConversationService, ConversationService>();
builder.Services.AddScoped<IRequestProcessingService, RequestProcessingService>();
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<IUserService, UserService>();



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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAnyOrigin");


app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<ExceptionHandler>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
