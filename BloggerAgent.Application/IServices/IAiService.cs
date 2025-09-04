using BloggerAgent.Application.Helpers;
using BloggerAgent.Domain.Commons;
using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloggerAgent.Application.IServices
{
    public interface IAIService
    {
        Task<string> GenerateReponse(string systemMessage, TaskContext context);
        Task<string> GenerateAsync(string systemPrompt, TaskContext taskRequest = null, IEnumerable<ChatMessageContent> messages = null, string userMessage = null);
        Task<string> ChatWithTools(TaskContext taskRequest, string systemPrompt, IEnumerable<ChatMessageContent> messages);
        Task<string> GenerateWebContentAsync(string systemMessage, TaskContext taskContext);
    }
}
