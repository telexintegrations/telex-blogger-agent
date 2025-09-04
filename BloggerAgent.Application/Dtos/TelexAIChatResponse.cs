using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloggerAgent.Application.Dtos
{
    public class TelexAIChatResponse
    {
        public string Status { get; set; }
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public TelexData Data { get; set; }
    }

    public class TelexData
    {
        public List<TelexChoice> Choices { get; set; }
        public long Created { get; set; }
        public string Id { get; set; }
        public string Model { get; set; }
        public string Object { get; set; }
        public string Provider { get; set; }
        public TelexUsage Usage { get; set; }
    }

    public class TelexChoice
    {
        public string FinishReason { get; set; }
        public int Index { get; set; }
        public object Logprobs { get; set; }
        public TelexMessage Message { get; set; }
        public string NativeFinishReason { get; set; }
    }

    public class TelexMessage
    {
        public string Content { get; set; }
        public string Role { get; set; }
        public List<TelexToolCall> ToolCalls { get; set; }
    }

    public class TelexToolCall
    {
        public string Id { get; set; }
        public int Index { get; set; }
        public string Type { get; set; }
        public TelexFunction Function { get; set; }
    }

    public class TelexFunction
    {
        public string Name { get; set; }
        public string Arguments { get; set; } // JSON string
    }

    public class TelexUsage
    {
        public int CompletionTokens { get; set; }
        public int PromptTokens { get; set; }
        public int TotalTokens { get; set; }
    }
}
