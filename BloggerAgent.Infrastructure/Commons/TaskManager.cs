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
            private readonly ITelexRepository<BlogTask> _taskRepo;
            private readonly ILogger<TaskManager> _logger;

            public TaskManager(ITelexRepository<BlogTask> taskRepo, ILogger<TaskManager> logger)
            {
                _taskRepo = taskRepo;
                _logger = logger;
            }

            public async Task<BlogTask?> ResolveAsync(string contextId, string userId)
            {
                var tasks = await _taskRepo.FilterAsync(new Dictionary<string, object>
            {
                { "contextId", contextId },
                { "userId", userId },
                { "status", Status.Active.ToString() }
            });

                return tasks?.OrderByDescending(t => t.LastActivityAt).FirstOrDefault();
            }

            public async Task<BlogTask> CreateNewAsync(string contextId, string userId, string? title = null)
            {
                var newTask = new BlogTask
                {
                    Id = Guid.NewGuid().ToString(),
                    ContextId = contextId,
                    UserId = userId,
                    Title = title ?? "Untitled Blog",
                    Status = Status.Active,
                    CurrentPhase = TaskPhase.Initialized,
                    LastActivityAt = DateTime.UtcNow,
                    History = new List<TaskPhase> { TaskPhase.Initialized }
                };

                await _taskRepo.CreateAsync(newTask);
                return newTask;
            }

            public async Task AdvancePhaseAsync(BlogTask task, TaskPhase nextPhase)
            {
                task.AdvancePhase(nextPhase);
                task.LastActivityAt = DateTime.UtcNow;
                await _taskRepo.UpdateAsync(task.Id, task);
            }

            public async Task UpdateContentAsync(BlogTask task, Blog updatedContent)
            {
                task.BlogContent = updatedContent;
                task.LastActivityAt = DateTime.UtcNow;
                await _taskRepo.UpdateAsync(task.Id, task);
            }

            public async Task MarkCompletedAsync(BlogTask task)
            {
                task.MarkCompleted();
                task.Status = Status.Completed;
                task.LastActivityAt = DateTime.UtcNow;
                await _taskRepo.UpdateAsync(task.Id, task);
            }

            public string GetProgressSummary(BlogTask task)
            {
                return $"Current phase: {task.CurrentPhase}, completed: {string.Join(", ", task.History)}";
            }
        }
    }
}
