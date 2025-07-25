using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloggerAgent.Application.Dtos.A2ATaskDtos
{
    public class TaskReceivedResponse
    {
        public string Jsonrpc { get; set; }
        public string Id { get; set; }
        public TaskReceivedResult Result { get; set; }
    }

    public class TaskReceivedResult
    {
        public string Id { get; set; }
        public Status Status { get; set; }
    }
}
