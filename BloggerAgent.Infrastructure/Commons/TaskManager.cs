using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloggerAgent.Infrastructure.Commons
{
    using global::BloggerAgent.Domain.Commons;
    using global::BloggerAgent.Domain.Enums;
    using global::BloggerAgent.Domain.IRepositories;
    using global::BloggerAgent.Domain.Models;
    using Microsoft.Extensions.Logging;

    namespace BloggerAgent.Infrastructure.Commons
    {
        public class TaskManager
        {
            private readonly ITelexRepository<Blog> _taskRepo;
            private readonly ILogger<TaskManager> _logger;

            public TaskManager(ITelexRepository<Blog> taskRepo, ILogger<TaskManager> logger)
            {
                _taskRepo = taskRepo;
                _logger = logger;
            }

            public async Task<Blog?> ResolveAsync(string contextId, string userId)
            {
                var tasks = await _taskRepo.FilterAsync(new Dictionary<string, object>
                {
                    { "contextId", contextId },
                    { "userId", userId },
                    { "status", Status.Active.ToString() }
                });

                return tasks?.OrderByDescending(t => t.UpdatedAt).FirstOrDefault();
            }

            public async Task<Blog> CreateNewAsync(string contextId, string userId, string? title = null)
            {
                var newTask = new Blog
                {
                    Id = Guid.NewGuid().ToString(),
                    ContextId = contextId,
                    UserId = userId,
                    Title = title ?? "Untitled Blog",
                    Status = Status.Active.ToString(),
                    CurrentPhase = TaskPhase.Initialized,
                    UpdatedAt = DateTime.UtcNow,
                    History = new List<TaskPhase> { TaskPhase.Initialized }
                };

                await _taskRepo.CreateAsync(newTask);
                return newTask;
            }

            public async Task AdvancePhaseAsync(Blog task, TaskPhase nextPhase)
            {
                task.CurrentPhase= nextPhase;
                task.UpdatedAt = DateTime.UtcNow;
                await _taskRepo.UpdateAsync(task.Id, task);
            }

            public async Task UpdateContentAsync(Blog taskBlog)
            {

                taskBlog.UpdatedAt = DateTime.UtcNow;
                await _taskRepo.UpdateAsync(taskBlog.Id, taskBlog);
            }

            public async Task MarkCompletedAsync(Blog task)
            {
                task.CurrentPhase = TaskPhase.Completed;
                task.Status = Status.Completed.ToString();
                task.UpdatedAt = DateTime.UtcNow;
                await _taskRepo.UpdateAsync(task.Id, task);
            }

            public string GetProgressSummary(Blog task)
            {
                return $"Current phase: {task.CurrentPhase}, completed: {string.Join(", ", task.History)}";
            }
        }
    }
}
