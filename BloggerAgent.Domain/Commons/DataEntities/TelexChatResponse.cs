using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloggerAgent.Domain.Commons.DataEntities
{
    public class TelexChatResponse
    {
        public TelexChatMessage Messages { get; set; } = new();
    }
}
