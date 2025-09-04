using BloggerAgent.Domain.Data;
using BloggerAgent.Domain.IRepositories;
using BloggerAgent.Application.IServices;
using BloggerAgent.Infrastructure.Repositories;
using BloggerAgent.Infrastructure.Services;
using BloggerAgent.Domain.DomainHelper;
using BloggerAgent.Infrastructure.Tooling;
using BloggerAgent.Domain.Repositories;
using BloggerAgent.Infrastructure.Tooling.Context;
using BloggerAgent.Infrastructure.Tooling.Types;
using BloggerAgent.Application.Configurations;
using BloggerAgent.Infrastructure.ToolFunctions;
using BloggerAgent.Infrastructure.Commons;
using BloggerAgent.Infrastructure.Commons.BloggerAgent.Infrastructure.Commons;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
namespace BloggerAgent.Infrastructure.Extensions
{
    public static class DIServices
    {
        public static void AddDIServices(this IServiceCollection services)
        {
            //Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT", "http://localhost:4318");
            //Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_PROTOCOL", "http/protobuf");
            //Environment.SetEnvironmentVariable("OTEL_SERVICE_NAME", "blogger-agent");
            services.AddScoped<DbContext>();
            services.AddScoped<HttpHelper>();
            services.AddHttpClient();

            services.AddScoped<DbContext>();
            services.AddScoped<HttpHelper>();           
            services.AddSingleton<BlogPlugin>();
            services.AddSingleton<AgentPlugin>();
            services.AddSingleton<OrganizationPlugin>();
            services.AddSingleton<TopicPlugin>();
            services.AddSingleton<ResearchPlugin>();
            services.AddSingleton<OutlinePlugin>();
            services.AddSingleton<KeywordPlugin>();
            services.AddSingleton<WriterPlugin>();
            services.AddSingleton<KernelProvider>();

            services.AddScoped<IBlogAgentService, BlogAgentService>();
            services.AddScoped<IResearchAgent, ResearchAgent>();
            services.AddScoped<OutlineAgent>();
            services.AddScoped<WriterAgent>();
            services.AddScoped<IBlogPostIntervalService, BlogPostIntervalService>();

            services.AddScoped(typeof(ITelexRepository<>), typeof(TelexRepositoryBase<>));
            services.AddScoped<IConversationRepository, ConversationRepository>();
            services.AddScoped<IBlogRepository, BlogRepository>();
            services.AddScoped<IRequestProcessingService, RequestProcessor>();
            services.AddScoped<IAIService, AIService>();
            services.AddScoped<ILlmTool, SaveOrganizationContextTool>();
            services.AddScoped<ILlmTool, GetOrganizationContextTool>();
            services.AddScoped<IOrganizationRepository, OrganizationRepository>();
            services.AddScoped<ToolRouter>();
            services.AddScoped<TaskContextAccessor>();
            services.AddScoped<IApiKeyRepository, ApiKeyRepository>();
            services.AddScoped<OrgApiKeyStore>();
            services.AddScoped<TaskManager>();

            //services.AddOpenTelemetry().WithTracing(tracerProviderBuilder =>
            //{
            //    tracerProviderBuilder
            //        .AddAspNetCoreInstrumentation()
            //        .AddHttpClientInstrumentation()
            //        .AddSource("Microsoft.SemanticKernel*")
            //        .AddConsoleExporter() // You can add other exporters here
            //        .AddOtlpExporter();
            //});

        }
    }
}
