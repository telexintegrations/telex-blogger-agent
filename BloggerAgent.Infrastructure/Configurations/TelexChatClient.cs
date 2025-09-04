using System.Text;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using System.Text.Json;
using BloggerAgent.Domain.Commons.constants;
using BloggerAgent.Application.Dtos;
using Microsoft.Extensions.Configuration;

namespace BloggerAgent.Infrastructure.Configurations
{
    public class TelexChatCompletionService : IChatCompletionService
    {
        private readonly HttpClient _httpClient;
        private readonly string _endpointUrl;
        private readonly string? _apiKey;

        public TelexChatCompletionService(HttpClient httpClient, string endpointUrl, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _endpointUrl = endpointUrl;
            _apiKey = configuration.GetValue<string>("TelexApiSettings:ApiKey");
        }

        public string ModelId => "google/gemini:2.5-flash:001";

        public IReadOnlyDictionary<string, object?> Attributes => new Dictionary<string, object?>();

        public async Task<IReadOnlyList<ChatMessageContent>> GetChatMessageContentsAsync(
            ChatHistory chatHistory,
            PromptExecutionSettings? requestSettings = null,
            Kernel? kernel = null,
            CancellationToken cancellationToken = default)
        {
            var payload = new
            {
                model = ModelId,
                messages = SerializeOutgoingMessages(chatHistory),
                tools = BuildTelexTools(kernel)
            };

            // ✅ Add API key to headers
            if (!_httpClient.DefaultRequestHeaders.Contains("X-AGENT-API-KEY"))
            {
                _httpClient.DefaultRequestHeaders.Add("X-AGENT-API-KEY", _apiKey);
            }

            var requestJson = JsonSerializer.Serialize(payload);
            var response = await _httpClient.PostAsync(
                _endpointUrl,
                new StringContent(requestJson, Encoding.UTF8, "application/json"),
                cancellationToken);

            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
            var telexResponse = JsonSerializer.Deserialize<TelexAIChatResponse>(responseJson);

            var results = new List<ChatMessageContent>();

            foreach (var choice in telexResponse.Data.Choices)
            {
                var msg = choice.Message;

                // Handle tool calls
                if (msg.ToolCalls != null && msg.ToolCalls.Count > 0)
                {
                    foreach (var toolCall in msg.ToolCalls)
                    {
                        // Parse arguments into KernelArguments
                        var rawArgs = JsonSerializer.Deserialize<Dictionary<string, object>>(toolCall.Function.Arguments);
                        var kernelArgs = new KernelArguments(rawArgs);

                        results.Add(
                            new()
                            {
                                Role = AuthorRole.Tool,
                                Items = [
                                    new FunctionCallContent(
                                    functionName: toolCall.Function.Name,
                                    pluginName: null, // Telex doesn't specify plugin name
                                    id: toolCall.Id,
                                    arguments: kernelArgs
                                    )
                                ]
                            }
                        );

                        return results;
                    }
                }

                // Handle regular assistant message
                else if (!string.IsNullOrWhiteSpace(msg.Content))
                {
                    results.Add(new ChatMessageContent(
                        AuthorRole.Assistant,
                        msg.Content
                     ));
                }
            }

            return results;
        }


        public IAsyncEnumerable<StreamingChatMessageContent> GetStreamingChatMessageContentsAsync(
            ChatHistory chatHistory,
            PromptExecutionSettings? executionSettings = null,
            Kernel? kernel = null,
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }


        private static List<object> SerializeOutgoingMessages(ChatHistory chatHistory)
        {
            var messages = new List<object>();
            string? role = default;
            foreach (var msg in chatHistory)
            {
                if (msg.Role == AuthorRole.System) role = Roles.System;
                else if (msg.Role == AuthorRole.User) role = Roles.User;
                else if (msg.Role == AuthorRole.Assistant) role = Roles.Assistant;
                else if (msg.Role == AuthorRole.Tool) role = Roles.Tool;

                // Handle function calls
                if (msg.Items.OfType<FunctionCallContent>().Any())
                {
                    foreach (var func in msg.Metadata.OfType<FunctionCallContent>())
                    {
                        messages.Add(new
                        {
                            role = Roles.Assistant,
                            tool_calls = new[]
                            {
                                new
                                {
                                    id = func.Id,
                                    type = "function",
                                    function = new
                                    {
                                        name = func.FunctionName,
                                        arguments = JsonSerializer.Serialize(func.Arguments)
                                    }
                                }
                            }
                        });
                    }
                    continue;
                }

                // Handle function results
                if (msg.Items.OfType<FunctionResultContent>().Any())
                {
                    foreach (var result in msg.Items.OfType<FunctionResultContent>())
                    {
                        messages.Add(new
                        {
                            role = "tool",
                            tool_call_id = result.CallId,
                            name = result.FunctionName,
                            content = result.Result
                        });
                    }
                    continue;
                }


                if (!string.IsNullOrWhiteSpace(msg.Content))
                {
                    messages.Add(new
                    {
                        role,
                        content = msg.Content
                    });
                }
                else 
                {
                    // Handle text and image content
                    var contentBuilder = new StringBuilder();
                    foreach (var item in msg.Items)
                    {
                        switch (item)
                        {
                            case TextContent text:
                                contentBuilder.AppendLine(text.Text);
                                break;
                            case ImageContent image:
                                contentBuilder.AppendLine($"![image]({image.Uri})"); // OpenRouter may not support this directly
                                break;
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(contentBuilder.ToString()))
                    {
                        messages.Add(new
                        {
                            role,
                            content = contentBuilder.ToString().Trim()
                        });
                    }
                    else
                    {
                        msg.Content = null;
                    }

                }
            }

            return messages;
        }


        public static List<object> BuildTelexTools(Kernel kernel)
        {
            var tools = new List<object>();

            foreach (var function in kernel.Plugins.GetFunctionsMetadata())
            {
                var properties = new Dictionary<string, object>();
                var required = new List<string>();

                foreach (var param in function.Parameters)
                {
                    var schema = param.Schema;

                    if (schema != null)
                    {
                        //var json = JsonSerializer.Serialize(schema);

                        var json = schema.RootElement.GetRawText();
                        var parsed = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
                        if (param.DefaultValue != null)
                            parsed["default"] = param.DefaultValue;
                        properties[param.Name] = parsed;
                    }

                    if (param.IsRequired)
                        required.Add(param.Name);
                }

                tools.Add(new
                {
                    type = "function",
                    function = new
                    {
                        name = $"{function.PluginName}-{function.Name}",
                        description = function.Description,
                        parameters = new
                        {
                            type = "object",
                            properties,
                            required
                        }
                    }
                });
            }

            return tools;
        }

        public static List<object> GetTelexToolsFromSK(Kernel kernel)
        {
            var tools = new List<object>();

            foreach (var function in kernel.Plugins.GetFunctionsMetadata())
            {
                var properties = new Dictionary<string, object>();
                var required = new List<string>();

                foreach (var param in function.Parameters)
                {
                    properties[param.Name] = ConvertToTelexParameter(param);
                    if (param.IsRequired)
                        required.Add(param.Name);
                }


                tools.Add(new
                {
                    type = "function",
                    function = new
                    {
                        name = $"{function.PluginName}-{function.Name}",
                        description = function.Description,
                        parameters = new
                        {
                            type = "object",
                            properties,
                            required
                        }
                    }
                });
            }

            return tools;
        }

        public static object ConvertToTelexParameter(KernelParameterMetadata param)
        {
            var jsonType = MapToJsonType(param.ParameterType ?? typeof(string));

            var schema = new Dictionary<string, object>
            {
                ["type"] = jsonType
            };

            if (!string.IsNullOrWhiteSpace(param.Description))
                schema["description"] = param.Description;

            if (param.DefaultValue != null)
                schema["default"] = param.DefaultValue;

            return schema;
        }

        private static string MapToJsonType(Type type)
        {
            if (type == typeof(string)) return "string";
            if (type == typeof(int) || type == typeof(long)) return "integer";
            if (type == typeof(float) || type == typeof(double) || type == typeof(decimal)) return "number";
            if (type == typeof(bool)) return "boolean";
            if (type == typeof(List<string>) || type == typeof(string[])) return "array";
            return "string"; // fallback
        }
    }
}
