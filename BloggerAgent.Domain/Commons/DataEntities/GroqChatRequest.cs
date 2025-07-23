using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloggerAgent.Domain.Commons.DataEntities
{
    public class GroqChatRequest
    {
        public string Model { get; set; } = "compound-beta";

        public List<Message> Messages { get; set; } = new();

        public class Message
        {
            public string Role { get; set; } = "user";
            public string Content { get; set; } = "";
        }
    }

}
