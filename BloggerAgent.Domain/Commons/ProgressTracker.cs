using BloggerAgent.Domain.Enums;
using BloggerAgent.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloggerAgent.Domain.Commons
{
    public class ProgressTracker
    {
        public string GetProgressSummary(Blog context)
        {
            return $"Current phase: {context.CurrentPhase}, completed: {string.Join(", ", context.History)}";
        }

        public void AdvancePhase(TaskPhase newPhase, Blog task)
        {
            task.CurrentPhase = newPhase;
            task.History.Add(newPhase);
        }

        public void InitializeTaskPhase(string title)
        {
            Blog blogTask = new Blog();
            blogTask.Id = Guid.NewGuid().ToString();
            blogTask.Status = Status.Active.ToString();

            blogTask.CurrentPhase = TaskPhase.Initialized;
        }

        public void MarkCompleted() => AdvancePhase(TaskPhase.Completed, new Blog());

        public bool HasReached(TaskPhase phase, Blog task) => task.History.Contains(phase);

      
    }
}
