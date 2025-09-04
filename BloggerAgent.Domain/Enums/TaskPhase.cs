using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloggerAgent.Domain.Enums
{
    public enum TaskPhase
    {
        Initialized,
        TopicGenerated,
        KeywordsGenerated,
        Researching,
        OutlineBuilt,
        ContentWritten,
        ImageGenerated,
        Completed,
        Failed
    }

}
