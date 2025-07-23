using BloggerAgent.Application.IServices;
using BloggerAgent.Domain.Commons;
using BloggerAgent.Domain.IRepositories;
using BloggerAgent.Domain.Models;
using BloggerAgent.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloggerAgent.Infrastructure.ToolFunctions
{
    public class BlogPlugin
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public BlogPlugin(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        [KernelFunction("save_blog_post")]
        [Description("Adds or save a particular blog post at the behest of the user.")]
        public async Task<string> SaveBlogPostAsync(string title, string content, string keywords = "")
        {
            using var scope = _scopeFactory.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<IBlogRepository>();

            var blog = new Blog
            {
                Title = title,
                Content = content,
                Keywords = { keywords },
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var success = await repo.AddBlogAsync(blog);
            return success
                ? $"📝 Blog post '{title}' saved successfully for org."
                : $"⚠️ Failed to save blog post '{title}'.";
        }

        [KernelFunction("update_blog_post")]
        [Description("Updates a previously saved blog post using its ID.")]
        public async Task<string> UpdateBlogPostAsync(string topic, Blog updatedBlog)
        {
            using var scope = _scopeFactory.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<IBlogRepository>();

            var success = await repo.UpdateBlogAsync(updatedBlog, topic);
            return success
                ? $"✏️ Blog post '{updatedBlog.Title}' updated successfully."
                : $"⚠️ Failed to update blog with topic '{topic}'.";
        }

        //[KernelFunction("list_blog_posts")]
        //[Description("Returns a list of all blog posts of an organization.")]
        //public async Task<List<Blog>> ListBlogPostsAsync()
        //{
        //    using var scope = _scopeFactory.CreateScope();
        //    var repo = scope.ServiceProvider.GetRequiredService<IBlogRepository>();

        //    var blogs = await repo.FilterByFieldAsync(nameof(BaseEntity.TagName), CollectionType.Blog);
        //    return blogs.Select(b => b).ToList();
        //}

        //[KernelFunction("summarize_blog_activity")]
        //[Description("Provides a summary of recent blog-related activity for a given organization (placeholder).")]
        //public async Task<string> GetBlogSummaryAsync(string organizationId)
        //{
        //    // Future: pull real metrics, counts, engagement stats
        //    return $"📊 Blog activity summary for org ID {organizationId}: [placeholder for posts, edits, keywords, etc.]";
        //}
    }
}
