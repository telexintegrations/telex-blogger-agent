using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloggerAgent.Application.Dtos.A2ATaskDtos
{
    public class AgentMessageResponse
    {
        public string Jsonrpc { get; set; }
        public string Id { get; set; }
        public TaskMessage Result { get; set; }
    }

  
}
