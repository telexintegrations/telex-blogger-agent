using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloggerAgent.Infrastructure.Tooling.Types
{
    public interface ILlmTool
    {
        string Name { get; }
        string Description { get; }
        Type ParameterType { get; }
        Task<ToolResult> ExecuteAsync(object parameters);
    }

}
