using BloggerAgent.Domain.Commons;
using Microsoft.AspNetCore.Http;
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

        public TaskContextAccessor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public TaskContext? GetTaskContext()
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
