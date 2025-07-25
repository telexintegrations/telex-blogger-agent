using BloggerAgent.Application.Contracts;
using BloggerAgent.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloggerAgent.Application.Dtos.A2ATaskDtos
{
    public class AgentTaskResponse
    {
        public string Jsonrpc { get; set; }
        public string Id { get; set; }
        public TaskResult Result { get; set; }
        public object? Error { get; set; }  // null here, but could be an error object if present

        public static object SendTaskResponse()
        {
            return new AgentTaskResponse
            {
                Jsonrpc = "2.0",
                Id = "c006266b7e954f2fb07eb02b26ce6d9e",
                Result = new TaskResult
                {
                    Id = "0195c514-2292-71e2-9378-21d11be2ad8c",
                    ContextId = "0195c514-2292-71e2-9378-21d11be2ad8c",
                    Status = new Status
                    {
                        State = State.Completed.ToString().ToLower(),
                        Timestamp = DateTime.Parse("2025-05-14T10:49:57.250107"),
                        Message = new TaskMessage
                        {
                            Role = "agent",
                            Parts = new List<TextPart>
                           {
                               new TextPart
                               {
                                   Kind = "text",
                                   Text = "Task completed with an artifact.",
                                   Metadata = null
                               }
                           },
                            Metadata = null
                        },
                    },
                    Artifacts = new()
                    {
                        new Artifact()
                        {
                            Name = "sample_artifact",
                            Parts = new List<TextPart>
                            {
                                new TextPart
                                {
                                    Kind = "text",
                                    Text = "This is sample content in the artifact",
                                    Metadata = null
                                },
                                //new DataPart
                                //{
                                //    Kind = "data",
                                //    Data = new Dictionary<string, object>
                                //    {
                                //        { "key", "value" },
                                //        { "example", 123 }
                                //    },
                                //    Metadata = null
                                //}
                            },
                            Metadata = new Dictionary<string, object>
                            {
                                { "created_at", "2025-05-15T11:56:41.856473" }
                            },
                            Append = null,
                            LastChunk = null

                        },
                    },
                    History = null,
                    Metadata = new Dictionary<string, object>
                    {
                        { "processed_at", "2025-05-15T11:56:41.856488" }
                    },
                    //Kind = "task"
                },
                Error = null
            };

        }
    }

    public class TaskResult
    {
        public string Id { get; set; }
        public string ContextId { get; set; }
        public Status Status { get; set; }
        public List<Artifact> Artifacts { get; set; }
        public List<TaskMessage> History { get; set; }
        public Dictionary<string, object>? Metadata { get; set; }
    }

    public class Status
    {
        public string State { get; set; }
        public DateTime Timestamp { get; set; }
        public TaskMessage Message { get; set; }
    }

    public class Artifact
    {
        public string ArtifactId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<TextPart> Parts { get; set; }
        public Dictionary<string, object>? Metadata { get; set; }
        public int? index { get; set; }
        public object? Append { get; set; }
        public object? LastChunk { get; set; }
    }
 
    public class TaskMessage
    {
        public string? TaskId { get; set; }
        public string MessageId { get; set; }
        public string ContextId { get; set; }
        public string Role { get; set; }
        public string? Kind { get; set; }
        public List<TextPart> Parts { get; set; }
        public Dictionary<string, object>? Metadata { get; set; }

    }
}

  
