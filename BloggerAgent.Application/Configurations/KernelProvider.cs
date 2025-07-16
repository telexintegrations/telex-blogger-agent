using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Google;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using BloggerAgent.Infrastructure.Services;
using OpenTelemetry.Logs;

namespace BloggerAgent.Application.Configurations
{
    public class KernelProvider
    {
        private readonly Kernel _kernel;
        private readonly IChatCompletionService _chatCompletionService;

        public KernelProvider(IConfiguration configuration)
        {
            var geminiApiKey = configuration.GetValue<string>("GeminiSetting:ApiKey")
                ?? throw new InvalidOperationException("Gemini API Key not configured!");

            var geminiModel = configuration.GetValue("GeminiSetting:Model", "gemini-1.5-pro");

            _kernel = BuildKernel(geminiModel, geminiApiKey);

            _chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>();
        }

        public Kernel Kernel => _kernel;

        public IChatCompletionService ChatCompletionService => _chatCompletionService;

        private static Kernel BuildKernel(string model, string apiKey)        
        {
            //Environment.SetEnvironmentVariable("SEMANTICKERNEL_EXPERIMENTAL_GENAI_ENABLE_OTEL_DIAGNOSTICS_SENSITIVE", "true");

            //var loggerFactory = LoggerFactory.Create(builder =>
            //{
            //    builder.AddOpenTelemetry(options =>
            //    {
            //        options.IncludeScopes = true;
            //        options.IncludeFormattedMessage = true;
            //        options.AddConsoleExporter();
            //    });
            //    builder.SetMinimumLevel(LogLevel.Trace);
            //});

            var kernelBuilder = Kernel.CreateBuilder()
                .AddGoogleAIGeminiChatCompletion(model, apiKey);
            //kernelBuilder.Services.AddSingleton(loggerFactory); // 🔌 Inject logger
            kernelBuilder.Services.AddLogging(logs => logs.AddConsole().SetMinimumLevel(LogLevel.Trace));


            return kernelBuilder.Build();
        }


        public void RegisterPlugins(IServiceProvider sp)
        {
            _kernel.Plugins.AddFromType<BlogAgentFunctions>("Organization", sp);
        }
    }

}
