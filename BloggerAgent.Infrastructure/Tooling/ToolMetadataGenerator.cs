using BloggerAgent.Infrastructure.Tooling.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BloggerAgent.Infrastructure.Tooling
{
    public static class ToolMetadataGenerator
    {
        public static ToolDescriptor DescribeTool(ILlmTool tool)
        {
            Type inputType = tool.ParameterType;
            PropertyInfo[] properties = inputType.GetProperties();

            var parameterProperties = new Dictionary<string, ToolParameter>();
            var requiredFields = new List<string>();

            foreach (PropertyInfo prop in properties)
            {
                string name = prop.Name;
                string type = MapDotNetTypeToJsonSchemaType(prop.PropertyType);
                string description = prop.GetCustomAttribute<DescriptionAttribute>()?.Description ?? "";

                // Check for [Required] or non-nullable value types
                bool isRequired = prop.GetCustomAttribute<RequiredAttribute>() != null 
                    || (prop.PropertyType.IsValueType && Nullable.GetUnderlyingType(prop.PropertyType) == null);

                if (isRequired)
                    requiredFields.Add(name);

                parameterProperties[name] = new ToolParameter
                {
                    Type = type,
                    Description = description
                };
            }

            return new ToolDescriptor
            {
                Name = tool.Name,
                Description = tool.Description,
                Parameters = new ToolParameterSchema
                {
                    Properties = parameterProperties,
                    Required = requiredFields
                }
            };
        }

        public static List<ToolDescriptor> DescribeTools(IEnumerable<ILlmTool> tools)
        {
            return tools.Select(DescribeTool).ToList();
        }

        private static string MapDotNetTypeToJsonSchemaType(Type type)
        {
            if (type == typeof(string)) return "string";
            if (type == typeof(int) || type == typeof(long)) return "integer";
            if (type == typeof(bool)) return "boolean";
            if (type == typeof(Enum)) return "enum";
            if (type == typeof(double) || type == typeof(float) || type == typeof(decimal)) return "number";
            if (type == typeof(List<Type>) || (type == typeof(Type[]))) return "array";
            if (type == typeof(DateTime)) return "string"; // as ISO-8601
            return "object";
        }
    }

}
