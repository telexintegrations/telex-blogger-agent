using BloggerAgent.Application.Helpers;
using BloggerAgent.Domain.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloggerAgent.Application.IServices
{
    public interface IAIService
    {
        Task<string> GenerateResponse(string message, string systemMessage, TaskContext blogDto);
        Task<string> ChatWithTools(TaskContext taskRequest);
    }
}
