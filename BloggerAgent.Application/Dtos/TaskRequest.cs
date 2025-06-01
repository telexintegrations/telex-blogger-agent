using BloggerAgent.Domain.Commons.Gemini;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloggerAgent.Application.Dtos
{
    public class TaskRequest
    {
        public string Jsonrpc { get; set; }
        public string Id { get; set; }
        public string Method { get; set; }
        public SendTaskParams Params { get; set; }
       
        public static object SendTaskRequest()
        {
            return new TaskRequest
            {
                Jsonrpc = "2.0",
                Id = "c006266b7e954f2fb07eb02b26ce6d9e",
                Method = "tasks/send",
                Params = new SendTaskParams
                {
                    Id = "0195c514-2292-71e2-9378-21d11be2ad8c",
                    SessionId = "0195c514-2292-71e2-9378-21d11be2ad8c",
                    Message = new Message
                    {
                        Role = "user",
                        Parts = new List<MessagePart>
                        {
                            new MessagePart
                            {
                                Type = "text",
                                Text = "Hello",
                                Metadata = null
                            }
                        },
                        Metadata = null
                    },
                    AcceptedOutputModes = null,
                    PushNotification = null,
                    HistoryLength = null,
                    Metadata = null
                }
            };

        }
    }

    public class SendTaskParams
    {
        public string Id { get; set; }           // task id
        public string SessionId { get; set; }    // session id (same as id here)
        public Message Message { get; set; }
        public List<string>? AcceptedOutputModes { get; set; }  // can be null
        public bool? PushNotification { get; set; }
        public int? HistoryLength { get; set; }
        public Dictionary<string, object>? Metadata { get; set; }
    }

    public class Message
    {
        public string Role { get; set; }  // e.g., "user"
        public List<MessagePart> Parts { get; set; }
        public Dictionary<string, object>? Metadata { get; set; }
    }

    public class MessagePart
    {
        public string Type { get; set; }   // e.g., "text"
        public string Text { get; set; }
        public Dictionary<string, object>? Metadata { get; set; }
    }

}
