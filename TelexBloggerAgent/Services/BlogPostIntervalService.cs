using TelexBloggerAgent.IServices;
using System.Timers;
using TelexBloggerAgent.Dtos;

namespace TelexBloggerAgent.Services
{
    public class BlogPostIntervalService : IBlogPostIntervalService
    {
        private readonly IBlogAgentService blogAgentService;
        private System.Timers.Timer _timer;

        public BlogPostIntervalService(IBlogAgentService blogAgentService)
        {
            this.blogAgentService = blogAgentService;
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
                Console.WriteLine($"Blog post scheduler started. Next post in {TimeSpan.FromMilliseconds(interval).TotalHours} hours.");
            }
            else
            {
                Console.WriteLine("Blog post scheduler stopped.");
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
            Console.WriteLine($"Blog post generated at {DateTime.Now}");
            this.blogAgentService.HandleAsync(blogPrompt);
        }
    }
}
