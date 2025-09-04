using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloggerAgent.Infrastructure.Tooling
{
    public class ToolResult
    {
        public string ToolName { get; set; }
        public object Output { get; set; }
        public string Status { get; set; }
    }
}
