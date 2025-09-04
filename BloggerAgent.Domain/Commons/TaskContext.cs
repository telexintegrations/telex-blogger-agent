using BloggerAgent.Domain.Commons.DataEntities;
using BloggerAgent.Domain.Enums;
using BloggerAgent.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloggerAgent.Domain.Commons
{
    public class TaskContext
    {
        public string Message { get; set; } = string.Empty;
        public string ContextId { get; set; } = string.Empty;
        public string? TaskId { get; set; }
        public string MessageId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string OrgId { get; set; } = string.Empty;

        public List<Setting> Settings { get; set; } = new();

        // Newly added fields
        public List<string> AcceptedOutputModes { get; set; } = new();
        public string CallbackUrl { get; set; } = string.Empty;
        public string AuthToken { get; set; } = string.Empty;
        public int HistoryLength { get; set; } = 0;
        public bool IsBlocking { get; set; } = false;

        // Optional: Telex Channel ID (for routing or broadcasting)
        public string ChannelId { get; set; } = string.Empty;
        public Blog? BlogTask { get; set; }
        public Company? Organization { get; set; }
        public List<TelexChatMessage> ChatMessages { get; set; } = new();
    }

   
}
