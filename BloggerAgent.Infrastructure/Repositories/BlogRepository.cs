using BloggerAgent.Domain.Commons;
using BloggerAgent.Domain.Commons.constants;
using BloggerAgent.Domain.Data;
using BloggerAgent.Domain.DomainHelper;
using BloggerAgent.Domain.IRepositories;
using BloggerAgent.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloggerAgent.Infrastructure.Repositories
{
    public class BlogRepository : TelexRepositoryBase<Blog>, IBlogRepository
    {

        private readonly ITelexRepository<Blog> _blogRepository;
        private readonly TaskContextAccessor _taskContext;

        public BlogRepository(ITelexRepository<Blog> blogRepository, DbContext context, TaskContextAccessor taskContext) : base(context)
        {
            _blogRepository = blogRepository;
            _taskContext = taskContext;
        }

        public TaskContext TaskContext => _taskContext.GetTaskContext();

        public async Task<bool> AddBlogAsync(Blog blogPost)
        {
            if (blogPost == null)
            {
                throw new ArgumentException("Blog cannot be null");
            }
            //blogPost.CurrentPhase = Domain.Enums.TaskPhase.Initialized;
            blogPost.CreatedAt = DateTime.Now;
            blogPost.ContextId = TaskContext.ContextId;
            blogPost.UserId = TaskContext.UserId;
            //blogPost.History.Add(Domain.Enums.TaskPhase.Initialized);

            return await _blogRepository.CreateAsync(blogPost);
        }
        
        
        public async Task<Blog> GetBlogAsync(string topic)
        {
            TaskContext taskContext = _taskContext.GetTaskContext();

            var filter = new Dictionary<string, object>
            {
                { "tag", CollectionType.Blog },
                { "contextId", taskContext.ContextId },
                { "user_id", taskContext.UserId }
            };

            if (topic != null)
            {
                filter.Add("title", topic.ToUpper());
            }

            var blogPosts = await FilterAsync(filter);
            var existingBlog = blogPosts.LastOrDefault();

            if (existingBlog == null)
            {
                return null;
            }

            return existingBlog;
        }

        public async Task<bool> UpdateBlogAsync(Dictionary<string, object> blogFieldsToUpdate, string topic)
        {
            if (blogFieldsToUpdate == null || !blogFieldsToUpdate.Any())
            {
                throw new ArgumentException("Update fields cannot be null or empty.");
            }

            TaskContext taskContext = _taskContext.GetTaskContext();

            var filter = new Dictionary<string, object>
            {
                { "tag", CollectionType.Blog },
                { "contextId", taskContext.ContextId },
                { "user_id", taskContext.UserId },
                {"title", topic.ToUpper() },
            };

            var blogPosts = await FilterAsync(filter);
            var existingBlog = blogPosts.FirstOrDefault();

            if (existingBlog == null)
            {
                throw new ArgumentException($"Blog with orgId {taskContext.OrgId} does not exist.");
            }

            foreach (var field in blogFieldsToUpdate)
            {
                switch (field.Key.ToLower())
                {                    
                    case "blogcontent":
                        existingBlog.BlogContent = field.Value?.ToString();
                        break;
                    case "keywords":
                        existingBlog.Keywords = field.Value as List<string> ?? new List<string>();
                        break;
                    case "outline":
                        existingBlog.Outline = field.Value?.ToString();
                        break;
                    case "referencelinks":
                        existingBlog.ReferenceLinks = field.Value as List<string> ?? new List<string>();
                        break;
                        // Add more fields if needed, except UpdatedAt
                }
            }

            // Always set updated timestamp to current time
            existingBlog.UpdatedAt = DateTime.UtcNow;

            return await _blogRepository.UpdateAsync(existingBlog.Id, existingBlog);
        }


    }
}
