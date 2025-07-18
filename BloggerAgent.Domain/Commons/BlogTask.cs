using BloggerAgent.Domain.Enums;
using BloggerAgent.Domain.IRepositories;
using BloggerAgent.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloggerAgent.Domain.Commons
{
    public class BlogTask : IEntity
    {
        public string Id { get; set; } 
        public string Title { get; set; }
        public string UserId { get; set; }
        public string ContextId { get; set; }
        public Status Status { get; set; }
        public TaskPhase CurrentPhase { get; set; } 
        public List<TaskPhase> History { get; set; } = new();
        public Blog BlogContent { get; set; }
        public DateTime LastActivityAt { get; set; }
        public string Tag { get; set; }

        public void AdvancePhase(TaskPhase phase)
        {
            CurrentPhase = phase;
            History.Add(phase);
        }
        
        public void InitializeTaskPhase(string title)
        {
            BlogTask blogTask = new BlogTask();
            blogTask.Id = Guid.NewGuid().ToString();
            blogTask.Status = Status.Active;

            blogTask.CurrentPhase = TaskPhase.Initialized;        
        }
       
        public void MarkCompleted() => AdvancePhase(TaskPhase.Completed);       

        public bool HasReached(TaskPhase phase) => History.Contains(phase);

        public string GetProgressSummary(TaskContext context)
        {
            return $"Current phase: {CurrentPhase}, completed: {string.Join(", ", History)}";
        }
    }
}
