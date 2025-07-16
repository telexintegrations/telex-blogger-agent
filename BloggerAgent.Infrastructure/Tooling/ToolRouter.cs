using BloggerAgent.Application.Helpers;
using BloggerAgent.Domain.Commons;
using BloggerAgent.Infrastructure.Tooling.Types;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BloggerAgent.Infrastructure.Tooling
{
    public class ToolRouter
    {
        private readonly Dictionary<string, ILlmTool> _tools;
        private readonly ILogger<ToolRouter> _logger;

        public ToolRouter(IEnumerable<ILlmTool> tools, ILogger<ToolRouter> logger)
        {
            _tools = tools.ToDictionary(t => t.Name, t => t);
            _logger = logger;
        }

        public async Task<ToolResult> ExecuteToolAsync(
            string toolName,
            object aiParameters, // typically null or JSON from AI
            TaskContext taskContext // your system's context from Telex
        )
        {
            if (!_tools.TryGetValue(toolName, out var tool))
            {
                _logger.LogWarning("Tool '{toolName}' not found", toolName);
                throw new InvalidOperationException($"Tool '{toolName}' not registered.");
            }

            _logger.LogInformation("Dispatching tool: {toolName}", toolName);

            object finalInput = null; 
            if (tool.ParameterType == typeof(GetOrganizationContextInput))
            {
                // Contextual input built by the backend
                finalInput = new GetOrganizationContextInput
                {
                    OrganizationId = taskContext.OrgId
                };
            }
            else if (aiParameters != null)
            {
                // General case: dynamically deserialize AI input to expected type
                var paramJson = JsonSerializer.Serialize(aiParameters);
                finalInput = JsonSerializer.Deserialize(paramJson, tool.ParameterType);
            }
            else
            {
                finalInput = null;
            }


            try
            {
                var result = await tool.ExecuteAsync(finalInput);
                _logger.LogInformation("Tool '{toolName}' executed successfully.", toolName);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing tool '{toolName}'", toolName);
                throw;
            }
        }

        public bool ShouldRunTool(string aiResponse)
        {
            try
            {
                using var doc = JsonDocument.Parse(aiResponse);
                return doc.RootElement.TryGetProperty("tool", out _);
            }
            catch
            {
                return false;
            }
        }

        public (string ToolName, object Parameters) ExtractToolInfo(string aiResponse)
        {
            using var doc = JsonDocument.Parse(aiResponse);
            var root = doc.RootElement;

            var toolName = root.GetProperty("tool").GetString();
            var parameters = root.TryGetProperty("parameters", out var paramElement) && paramElement.ValueKind != JsonValueKind.Null
                ? JsonSerializer.Deserialize<object>(paramElement.GetRawText())
                : null;

            return (toolName, parameters);
        }

        public string FormatToolResult(string toolName, object result)
        {
            return JsonSerializer.Serialize(new
            {
                tool_result = new
                {
                    tool = toolName,
                    output = result
                }
            });
        }

        public string BuildSystemMessage()
        {
            var toolDescriptions = ToolMetadataGenerator.DescribeTools(_tools.Values);
            var toolJson = JsonSerializer.Serialize(toolDescriptions, new JsonSerializerOptions { WriteIndented = true });

            return $$$"""
                You are a blogging assistant designed to help users generate high-quality blog content.

                You have access to special tools that let you fetch or store contextual data. When you need to perform a specific action, you must return a JSON object with the tool call.

                ---

                📌 To use a tool, reply in this exact format:
                {"tool": "tool_name",
                  "parameters": {
                    "param1": "value1",
                    "param2": "value2"
                  }
                }

                📥 Tool parameter types and descriptions are provided below. Only return the JSON structure above—no surrounding text.

                ---

                ✅ When a tool is executed, its result will be passed back to you in this format:
                {
                  "tool_result": {
                    "tool": "tool_name",
                    "output": {... // result from tool
                    }
                  }
                }

                You should then use this result to respond meaningfully to the user or decide if another tool is needed.

                ---

                🧰 Available Tools:
                {{{toolJson}}}

                Tool calls are ultimately meant for the backend system not the user. Only when you have gathered back all the information you need can you decide to respond back to the user. The user should not know anything about your toolcall processes. If there is an error, you can just respond back to the user with a natural language error message.

                """;
        }

    }

}
