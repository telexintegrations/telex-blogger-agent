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

        [KernelFunction("create_blog_post")]
        [Description("Creates or adds a particular blog post at the behest of the user.")]
        public async Task<string> SaveBlogPostAsync(string title)
        {
            using var scope = _scopeFactory.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<IBlogRepository>();

            var existingBlog = await repo.GetBlogAsync(title);
            if (existingBlog != null)
            {
                return $"Blog post with title {existingBlog.Title} already exists";
            }

            var blog = new Blog
            {
                Title = title.ToUpper(),
            };

            var success = await repo.AddBlogAsync(blog);
            return success
                ? $"📝 Blog post '{title}' saved successfully for org."
                : $"⚠️ Failed to save blog post '{title}'.";
        }

        [KernelFunction("update_blog_post")]
        [Description("Updates blog post as it progresses.")]
        public async Task<string> UpdateBlogPostAsync([Description("One or more blog post fields to be updated (eg: Keywords:array, Outline:string, BlogContent:string,ReferenceLinks:array")]Dictionary<string, object> fieldsToUpdate, [Description("Blog title to filter with")]string topic)
        {
            using var scope = _scopeFactory.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<IBlogRepository>();

            var success = await repo.UpdateBlogAsync(fieldsToUpdate, topic);
            return success
                ? $"✏️ Blog post '{topic}' updated successfully."
                : $"⚠️ Failed to update blog with topic '{topic}'.";
        }

    }
}
