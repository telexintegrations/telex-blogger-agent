using BloggerAgent.Application.Contracts;
using BloggerAgent.Application.Dtos.A2ATaskDtos;
using BloggerAgent.Domain.Commons;
using BloggerAgent.Domain.Commons.DataEntities;
using BloggerAgent.Domain.Enums;
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

        public static AgentMessageResponse ConstructResponse(A2aTaskRequest request, string response)
        {
            return new AgentMessageResponse
            {
                Jsonrpc = request.Jsonrpc,
                Id = request.Id,
                Result = new TaskMessage()
                {
                    Role = "agent",
                    Kind = "message",
                    MessageId = Guid.NewGuid().ToString(),
                    ContextId = request.Params.Message.ContextId,
                    Parts = new List<ITaskPart>
                    {
                        new TextPart
                        {
                            Kind = "text",
                            Text = response,
                        }
                    },
                    Metadata = null

                }
            };
        }
        
        public static TaskReceivedResponse ConstructTaskReceivedResponse(A2aTaskRequest request)
        {
            return new TaskReceivedResponse
            {
                Jsonrpc = request.Jsonrpc,
                Id = request.Id,
                Result = new()
                {
                    Id = request.Params.Message.TaskId ?? Guid.NewGuid().ToString(),
                    Status = new()
                    {
                        State = State.Submitted.ToString().ToLower(),
                        Message = new TaskMessage
                        {
                            MessageId = Guid.NewGuid().ToString(),
                            Role = "agent",
                            Kind = "message",
                            Parts = new List<ITaskPart>
                            {
                                new TextPart
                                {
                                    Kind = "text",
                                    Text = "Message recieved successfully",
                                }
                            }
                        }
                    }

                }
            };
        }

        public static AgentTaskResponse ConstructPushNotificationTask(A2aTaskRequest request, string response, string taskId)
        {
            var contextId = request.Params.Message.ContextId;

            return new AgentTaskResponse
            {
                Jsonrpc = request.Jsonrpc,
                Id = request.Id,
                Result = new TaskResult
                {
                    Id = taskId,
                    ContextId = contextId,
                    Status = new Dtos.A2ATaskDtos.Status
                    {
                        State = State.Completed.ToString().ToLower(),
                        Timestamp = DateTime.UtcNow,
                        Message = new TaskMessage
                        {
                            Role = "agent",
                            MessageId = Guid.NewGuid().ToString(),
                            Kind = "message",
                            Parts = new List<ITaskPart>
                            {
                                new TextPart
                                {
                                    Text = $"Response generated successfully",                                    
                                }
                            },
                        }
                    },
                    Artifacts = new List<Artifact>
                    {   new Artifact
                        {
                            ArtifactId = Guid.NewGuid().ToString(),
                            Name = "push_notification_artifact",
                            Parts = new List<ITaskPart>
                            {
                                new TextPart
                                {
                                    Text = response,
                                    
                                }
                            },
                        }
                    },
                }
            };
        }

        public static AgentTaskResponse ConstructPushNotificationTaskWithFile(A2aTaskRequest request, string response, string taskId)
        {
            var contextId = request.Params.Message.ContextId;

            return new AgentTaskResponse
            {
                Jsonrpc = request.Jsonrpc,
                Id = request.Id,
                Result = new TaskResult
                {
                    Id = taskId,
                    ContextId = contextId,
                    Status = new Dtos.A2ATaskDtos.Status
                    {
                        State = State.Completed.ToString().ToLower(),
                        Timestamp = DateTime.UtcNow,
                        Message = new TaskMessage
                        {
                            Role = "agent",
                            MessageId = Guid.NewGuid().ToString(),
                            Kind = "message",
                            Parts = new List<ITaskPart>
                            {
                                new TextPart
                                {
                                    Text = "Task Completed Successfully"
                                }
                            },
                        }
                    },
                    Artifacts = new List<Artifact>
                    {
                        new Artifact
                        {
                            ArtifactId = Guid.NewGuid().ToString(),
                            Name = "push_notification_artifact",
                            Parts = new List<ITaskPart>
                            {
                                new TextPart
                                {
                                    Text = response
                                },
                                new FilePart
                                {
                                    File = new FileContent
                                    {
                                        Name = "notification-image.png",
                                        MimeType = "image/png",
                                        Url = "https://media.istockphoto.com/id/926196952/photo/beautiful-nature-background.jpg?s=1024x1024&w=is&k=20&c=EKZIvj3y_le8HiWP4Vg58dtDfMp8Zuaj8g77v-bLjPw=",
                                        Bytes = ""
                                        // Optionally: Bytes = Convert.ToBase64String(...)
                                    },
                                }
                            }
                        }
                    }
                }
            };
        }




        public static TaskContext ExtractTaskData(A2aTaskRequest request)
        {
            var message = request?.Params?.Message;
            var config = request?.Params?.Configuration;
            var pushConfig = config?.PushNotificationConfig;
            var metadata = message?.Metadata ?? new Dictionary<string, object>();
            var fallbackContextId = pushConfig?.Url?.TrimEnd('/')?.Split('/')?.LastOrDefault();
            string channelId = metadata.TryGetValue("telex_channel_id", out var channel) ? channel?.ToString() ?? string.Empty : string.Empty;

            if (message == null || message.Parts == null || !message.Parts.Any())
                throw new ArgumentException("Invalid message structure");
            var part = message.Parts.FirstOrDefault() as TextPart;
            return new TaskContext
            {
                Message = part?.Text ?? string.Empty,
                ContextId = message.ContextId ?? channelId ?? fallbackContextId ?? string.Empty,
                TaskId = message.TaskId,
                MessageId = message.MessageId ?? string.Empty,
                OrgId = metadata.TryGetValue("org_id", out var org) ? org?.ToString() ?? string.Empty : string.Empty,
                UserId = metadata.TryGetValue("telex_user_id", out var user) ? user?.ToString() ?? string.Empty : string.Empty,
                ChannelId = channelId,
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
