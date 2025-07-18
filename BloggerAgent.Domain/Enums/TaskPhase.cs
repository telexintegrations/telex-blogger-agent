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
        Researching,
        TopicGenerated,
        KeywordsGenerated,
        OutlineBuilt,
        ContentWritten,
        ImageGenerated,
        Completed,
        Failed
    }

}
