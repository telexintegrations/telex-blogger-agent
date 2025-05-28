using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using TelexBloggerAgent.Dtos;
using TelexBloggerAgent.Helpers;
using TelexBloggerAgent.IServices;
using TelexBloggerAgent.Models;
using TelexBloggerAgent.Services;

namespace TelexBloggerAgent.test.Services
{

    public class BlogAgentServiceTests
    {
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
        private readonly HttpClient _httpClient;
        private readonly Mock<ILogger<BlogAgentService>> _loggerMock;
        private readonly Mock<IOptions<GeminiSetting>> _geminiSettingsMock;
        private readonly BlogAgentService _blogAgentService;
        private readonly Mock<IOptions<TelexSetting>> _telexSettingsMock;
        private readonly Mock<IRequestProcessingService> _requestServiceMock;
        private readonly Mock<IConversationService> _conversationServiceMock;

        private const string Identifier = "📝 #TelexBlog";
        private const string MockGeminiUrl = "https://mock-gemini.com";
        private const string MockApiKey = "mock-api-key";

        public BlogAgentServiceTests()
        {
            _httpClient = new HttpClient(_httpMessageHandlerMock.Object) 
            { 
                BaseAddress = new Uri(MockGeminiUrl) 
            };

            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            _httpClientFactoryMock = new Mock<IHttpClientFactory>();
            _httpClientFactoryMock.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(_httpClient);
            _loggerMock = new Mock<ILogger<BlogAgentService>>();
            _geminiSettingsMock = new Mock<IOptions<GeminiSetting>>();
            _geminiSettingsMock.Setup(gs => gs.Value).Returns(new GeminiSetting { ApiKey = MockApiKey, GeminiUrl = MockGeminiUrl });

            _blogAgentService = new BlogAgentService(
                _httpClientFactoryMock.Object,
                _geminiSettingsMock.Object,
                _telexSettingsMock.Object,
                _loggerMock.Object,
                _requestServiceMock.Object,
                _conversationServiceMock.Object
            );
        }



        [Fact]
        public async Task GenerateBlogAsync_ShouldThrowErrorOnFailure()
        {

            var blogPrompt = new GenerateBlogDto { Message = "Write about C#" };

            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new HttpRequestException("API failure"));

            await Assert.ThrowsAsync<ArgumentNullException>(() => _blogAgentService.HandleAsync(blogPrompt));
            
        }

        [Fact]
        public async Task GenerateBlogAsync_ShouldCallApiAndProcessResponse()
        {
            var blogPrompt = new GenerateBlogDto 
            { 
                Message = "Write about technology", 
                Settings = new List<Setting>
                { 
                   new Setting { Label = "webhook_url", Default = "https://webhook.urL" }
                } 
            };


            var apiResponse = @"{ ""candidates"": [ { ""content"": { ""parts"": [ { ""text"": ""Generated Blog Content"" } ] } } ] }";

            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.OK | HttpStatusCode.Accepted, Content = new StringContent(apiResponse) });

            await _blogAgentService.HandleAsync(blogPrompt);

            _loggerMock.Verify(logger =>
                    logger.Log(
                        It.Is<LogLevel>(level => level == LogLevel.Information),
                        It.IsAny<EventId>(),
                        It.Is<It.IsAnyType>((state, _) => state.ToString().Contains("Blog post generated successfully")),
                        It.IsAny<Exception>(),
                        It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                    Times.Once);
        }


        [Fact]
        public async Task GenerateBlogAsync_ShouldSkipIfMessageContainsIdentifier()
        {
            var blogPrompt = new GenerateBlogDto { Message = "Some blog content " + Identifier };

            await _blogAgentService.HandleAsync(blogPrompt);

            _httpMessageHandlerMock.Protected()
                .Verify("SendAsync", Times.Never(), ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>());
        }


        [Fact]
        public async Task SendBlogAsync_ShouldThrowExceptionWhenWebhookUrlIsMissing()
        {
            var channelId = "";

            var settings = new List<Setting> { new Setting { Label = "some_other_setting", Default = "value" } };

            await Assert.ThrowsAsync<Exception>(() => _blogAgentService.SendResponseAsync("Generated Blog", channelId));
        }


        [Fact]
        public async Task SendBlogAsync_ShouldCallTelexWebhook()
        {
            var channelId = "";
            var settings = new List<Setting> { new Setting { Label = "webhook_url", Default = "https://telex.com/webhook" } };

            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.Accepted });

            await _blogAgentService.SendResponseAsync("Generated Blog", channelId);

            _loggerMock.Verify(logger =>
                logger.Log(
                    It.Is<LogLevel>(level => level == LogLevel.Information),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((state, _) => state.ToString().Contains($"Blog post successfully sent to telex")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

    }
}
