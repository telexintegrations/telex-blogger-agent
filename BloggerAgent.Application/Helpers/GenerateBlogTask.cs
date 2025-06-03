using System.Text.Json;
using System.Text.Json.Serialization;
using BloggerAgent.Application.Dtos;
using BloggerAgent.Domain.Commons;

namespace BloggerAgent.Application.Helpers
{
    public class GenerateBlogTask
    {       
        public string Message { get; set; }
        
        public string ContextId { get; set; } 
        
        public string? TaskId { get; set; }

        public string MessageId { get; set; }

        public List<Setting>? Settings { get; set; } = new();

        public static GenerateBlogTask MapToGenerateBlogDto(TaskRequest request)
        {
            var message = request?.Params?.Message;

            if (message == null || message.Parts == null || !message.Parts.Any())
                throw new ArgumentException("Invalid message structure");

            return new GenerateBlogTask
            {
                Message = message.Parts.First().Text,
                ContextId = message.ContextId,
                TaskId = message.TaskId,
                MessageId = message.MessageId,
                //OrganizationId = request.Params.Metadata != null &&
                //                 request.Params.Metadata.TryGetValue("org_id", out var org)
                //                 ? org?.ToString()
                //                 : null,
                Settings = request.Params.Metadata != null &&
                           request.Params.Metadata.TryGetValue("settings", out var settingsObj)
                           ? JsonSerializer.Deserialize<List<Setting>>(settingsObj.ToString())
                           : new List<Setting>()
            };
        }
    }
}
