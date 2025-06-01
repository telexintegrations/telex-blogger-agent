using BloggerAgent.Application.IServices;
using System.Timers;
using Microsoft.Extensions.Logging;
using BloggerAgent.Application.Dtos;

namespace BloggerAgent.Infrastructure.Services
{
    public class BlogPostIntervalService : IBlogPostIntervalService
    {
        private readonly IBlogAgentService blogAgentService;
        private readonly ILogger<BlogPostIntervalService> _logger;
        private System.Timers.Timer _timer;


        public BlogPostIntervalService(IBlogAgentService blogAgentService, ILogger<BlogPostIntervalService> logger)
        {
            this.blogAgentService = blogAgentService;
            _logger = logger;
        }

        public void ScheduleBlogPostGeneration(string option, GenerateBlogDto blogPrompt)
        {
            double interval = ConvertOptionToInterval(option);

            // Stop existing timer if running
            _timer?.Stop();
            _timer?.Dispose();
            _timer = null;

            if (interval > 0)
            {
                _timer = new System.Timers.Timer(interval);
                _timer.Elapsed += (sender, e) => GenerateBlogPost(blogPrompt);
                _timer.AutoReset = true;
                _timer.Start();
                _logger.LogInformation($"Blog post scheduler started. Next post in {TimeSpan.FromMilliseconds(interval).TotalHours} hours.");
            }
            else
            {
                _logger.LogInformation("Blog post scheduler stopped.");
                return;
            }
        }

        private double ConvertOptionToInterval(string option)
        {
            return option switch
            {
                "Every 12 hours" => TimeSpan.FromHours(12).TotalMilliseconds,
                "Every 24 hours" => TimeSpan.FromHours(24).TotalMilliseconds,
                _ => 0 // "None" or any invalid input
            };
        }

        private void GenerateBlogPost(GenerateBlogDto blogPrompt)
        {
            _logger.LogInformation($"Blog post generated at {DateTime.Now}");
            this.blogAgentService.HandleAsync(blogPrompt);
        }
    }
}
