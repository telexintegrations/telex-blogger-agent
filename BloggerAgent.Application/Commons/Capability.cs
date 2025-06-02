using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloggerAgent.Application.Commons
{
    public class Capability
    {
        public bool Streaming { get; set; }
        public bool PushNotifications { get; set; }
        public bool StateTransitionHistory { get; set; }
    }
}
