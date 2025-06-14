using BloggerAgent.Application.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloggerAgent.Application.IServices
{
    public interface IAiService
    {
        Task<string> GenerateResponse(string message, string systemMessage, GenerateBlogTask blogDto);
    }
}
