using TelexBloggerAgent.Helpers;
using TelexBloggerAgent.IServices;
using TelexBloggerAgent.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IBlogAgentService, BlogAgentService>();
builder.Services.AddScoped<ITelexIngegrationService, TelexIntegrationService>();

builder.Services.Configure<GeminiSetting>(builder.Configuration.GetSection("GeminiSetting"));
builder.Services.Configure<TelexSetting>(builder.Configuration.GetSection("TelexSetting"));

builder.Services.AddCors(options =>
{
    options.AddPolicy("TelexPolicy", policy =>
    {
        policy.WithOrigins(
            "https://telex.im",
            "http://staging.telextest.im",
            "http://telextest.im",
            "https://staging.telex.im")
               .AllowAnyHeader()
               .AllowAnyMethod();
    });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("TelexPolicy");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
