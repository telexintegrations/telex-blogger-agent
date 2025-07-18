using BloggerAgent.Application.Dtos.A2ATaskDtos;
using BloggerAgent.Domain.Commons;
using BloggerAgent.Domain.Commons.DataEntities;
using BloggerAgent.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BloggerAgent.Application.Helpers
{
    public class DataExtract
    {
        public static string? ExtractAiResponseData(string responseString)
        {
            var responseJson = JsonSerializer.Deserialize<JsonElement>(responseString);

            // Check if the response contains candidates
            if (!responseJson.TryGetProperty("candidates", out var candidates) && candidates.GetArrayLength() > 0)
            {               
                throw new Exception("Invalid API response: No candidates.");
            }

            // Extract the generated response from the first candidate
            var generatedResponse = candidates[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString();
            return generatedResponse;
        }

        public Company ExtractCompanyDetails(TaskContext blogDto)
        {

            // Retrieve settings dynamically
            string companyName = GetSettingValue(blogDto.Settings, "company_name");
            string companyOverview = GetSettingValue(blogDto.Settings, "company_overview");
            string companyWebsite = GetSettingValue(blogDto.Settings, "company_website");
            string tone = GetSettingValue(blogDto.Settings, "tone");
            string blogLength = GetSettingValue(blogDto.Settings, "blog_length");
            string format = GetSettingValue(blogDto.Settings, "format");
            string targetAudience = GetSettingValue(blogDto.Settings, "target_audience");
            string industry = GetSettingValue(blogDto.Settings, "industry");

            return new Company
            {
                Name = companyName,
                Industry = industry,
                Tone = tone,
                Overview = companyOverview,
                Website = companyWebsite,
                TargetAudience = targetAudience
            };

        }

        public static string GetSettingValue(List<Setting> settings, string key)
        {
            return settings.FirstOrDefault(s => s.Label == key)?.Default.ToString() ?? "";
        }

        public static MessageResponse ConstructResponse(TaskRequest request, string response)
        {
            return new MessageResponse
            {
                Jsonrpc = "2.0",
                Id = request.Id,
                Result = new ResponseMessage()
                {
                    Role = "agent",
                    Kind = "message",
                    MessageId = Guid.NewGuid().ToString(),
                    TaskId = request.Params.Message.TaskId,
                    ContextId = request.Params.Message.ContextId,
                    Parts = new List<MessageResponsePart>
                    {
                        new MessageResponsePart
                        {
                            Kind = "text",
                            Text = response,
                        }
                    },
                    Metadata = null

                }
            };
        }

        public static TaskContext ExtractTaskData(TaskRequest request)
        {
            var message = request?.Params?.Message;
            var config = request?.Params?.Configuration;
            var pushConfig = config?.PushNotificationConfig;
            var metadata = message?.Metadata ?? new Dictionary<string, object>();

            if (message == null || message.Parts == null || !message.Parts.Any())
                throw new ArgumentException("Invalid message structure");

            return new TaskContext
            {
                Message = message.Parts.First().Text ?? string.Empty,
                ContextId = message.ContextId ?? string.Empty,
                TaskId = message.TaskId,
                MessageId = message.MessageId ?? string.Empty,
                OrgId = metadata.TryGetValue("org_id", out var org) ? org?.ToString() ?? string.Empty : string.Empty,
                UserId = metadata.TryGetValue("telex_user_id", out var user) ? user?.ToString() ?? string.Empty : string.Empty,
                ChannelId = metadata.TryGetValue("telex_channel_id", out var channel) ? channel?.ToString() ?? string.Empty : string.Empty,
                Settings = metadata.TryGetValue("settings", out var settingsObj) && settingsObj != null
                           ? JsonSerializer.Deserialize<List<Setting>>(settingsObj.ToString()!) ?? new List<Setting>()
                           : new List<Setting>(),

                AcceptedOutputModes = config?.AcceptedOutputModes ?? new List<string>(),
                CallbackUrl = pushConfig?.Url ?? string.Empty,
                AuthToken = pushConfig?.Authentication?.Credentials ?? string.Empty,
                HistoryLength = config?.HistoryLength ?? 0,
                IsBlocking = config?.Blocking ?? false
            };
        }
    }
}
