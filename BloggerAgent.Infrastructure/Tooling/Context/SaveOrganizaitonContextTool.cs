using BloggerAgent.Domain.IRepositories;
using BloggerAgent.Domain.Models;
using BloggerAgent.Infrastructure.Tooling.Types;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace BloggerAgent.Infrastructure.Tooling.Context
{
    public class SaveOrganizationContextTool : ILlmTool
    {
        private readonly IOrganizationRepository _companyService;
        private readonly ILogger<SaveOrganizationContextTool> _logger;

        public SaveOrganizationContextTool(IOrganizationRepository companyService, ILogger<SaveOrganizationContextTool> logger)
        {
            _companyService = companyService;
            _logger = logger;
        }

        public string Name => "saveOrganizationContext";
        public string Description => "Saves the user's company profile and preferences to personalize future blog posts.";
        public Type ParameterType => typeof(SaveOrganizationContextInput);

        public async Task<ToolResult> ExecuteAsync(object parameters)
        {
            _logger.LogInformation("Tool '{ToolName}' invoked with parameters of type {ParamType}", Name, parameters?.GetType().Name ?? "null");

            if (parameters is not SaveOrganizationContextInput input)
            {
                _logger.LogWarning("Tool '{ToolName}' received invalid parameters.", Name);
                return new ToolResult
                {
                    ToolName = Name,
                    Status = "error",
                    Output = "Invalid parameters: expected SaveOrganizationContextInput"
                };
            }

            var company = new Company
            {
                Name = input.Name,
                TargetAudience = input.TargetAudience,
                Tone = input.Tone,
                Industry = input.Industry,
                Overview = input.Overview,
                Website = input.Website,
                CreatedAt = DateTime.UtcNow,
            };

            try
            {
                _logger.LogInformation("Attempting to persist organization '{OrgName}' in industry '{Industry}'", company.Name, company.Industry);

                bool saved = await _companyService.CreateAsync(company);

                if (saved)
                {
                    _logger.LogInformation("Organization '{OrgName}' saved successfully.", company.Name);
                    return new ToolResult
                    {
                        ToolName = Name,
                        Status = "success",
                        Output = "Organization saved successfully."
                    };
                }
                else
                {
                    _logger.LogError("Failed to save organization '{OrgName}'. Repository returned false.", company.Name);
                    return new ToolResult
                    {
                        ToolName = Name,
                        Status = "failure",
                        Output = "Failed to save organization."
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while saving organization '{OrgName}'", company.Name);
                return new ToolResult
                {
                    ToolName = Name,
                    Status = "error",
                    Output = "An unexpected error occurred while saving organization."
                };
            }
        }
    }
}