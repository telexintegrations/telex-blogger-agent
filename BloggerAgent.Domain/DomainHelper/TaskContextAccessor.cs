using BloggerAgent.Domain.Commons;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloggerAgent.Domain.DomainHelper
{
    public class TaskContextAccessor 
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMemoryCache _cache;

        private static readonly AsyncLocal<TaskContext> _current = new();

        public static TaskContext Current
        {
            get => _current.Value;
            set => _current.Value = value;
        }

        public TaskContext GetTaskContext()
        {
            return _current.Value;
        }
        
        public void SetTaskContext(TaskContext taskContext)
        {
            _current.Value = taskContext;
        }


        public TaskContextAccessor(IHttpContextAccessor httpContextAccessor, IMemoryCache cache)
        {
            _httpContextAccessor = httpContextAccessor;
            _cache = cache;
        }



        public void Save(string taskId, TaskContext context, TimeSpan? ttl = null)
        {
            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = ttl ?? TimeSpan.FromMinutes(10)
            };

            _cache.Set(taskId, context, options);
        }

        public TaskContext Get(string taskId)
        {
            _cache.TryGetValue(taskId, out TaskContext context);
            return context;
        }

        public void Remove(string taskId)
        {
            _cache.Remove(taskId);
        }




        public TaskContext? GetTaskContet()
        {
            var context = _httpContextAccessor.HttpContext;
            if (context?.Items.TryGetValue("TaskContext", out var value) == true)
            {
                return value as TaskContext;
            }

            return null;
        }
    }
}
