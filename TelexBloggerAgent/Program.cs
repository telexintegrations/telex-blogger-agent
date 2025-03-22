using TelexBloggerAgent.Data;
using TelexBloggerAgent.Helpers;
using TelexBloggerAgent.IRepositories;
using TelexBloggerAgent.IServices;
using TelexBloggerAgent.Repositories;
using TelexBloggerAgent.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Bind MongoDB settings

builder.Services.Configure<MongoDbConfig>(builder.Configuration.GetSection("MongoDbConfig"));

builder.Services.Configure<GeminiSetting>(builder.Configuration.GetSection("GeminiSetting"));
builder.Services.Configure<TelexSetting>(builder.Configuration.GetSection("TelexSetting"));

builder.Services.Configure<MongoDbConfig>(builder.Configuration.GetSection("MongoDbSettings"));
builder.Services.AddSingleton<MongoDbContext>();

builder.Services.AddHttpClient();
builder.Services.AddTransient<IBlogAgentService, BlogAgentService>();

builder.Services.AddScoped<ITelexIntegrationService, TelexIntegrationService>();
builder.Services.AddScoped<IConversationRepository, ConversationRepository>();
builder.Services.AddScoped<IConversationService, ConversationService>();
builder.Services.AddScoped<IRequestProcessingService, RequestProcessingService>();



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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
