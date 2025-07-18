using BloggerAgent.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloggerAgent.Domain.Commons
{
    public class ProgressTracker
    {
        public TaskPhase CurrentPhase { get; private set; } = TaskPhase.Initialized;
        public List<TaskPhase> History { get; private set; } = new();

        public void Advance(TaskPhase newPhase)
        {
            CurrentPhase = newPhase;
            History.Add(newPhase);
        }

        public bool HasReached(TaskPhase phase) => History.Contains(phase);

        public string GetProgressSummary(TaskContext context)
        {
            return $"Current phase: {CurrentPhase}, completed: {string.Join(", ", History)}";
        }
    }
}
