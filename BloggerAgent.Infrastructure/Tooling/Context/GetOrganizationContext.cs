using System;
using System.Linq;
using System.Threading.Tasks;
using BloggerAgent.Domain.IRepositories;
using BloggerAgent.Infrastructure.Tooling.Types;
using Microsoft.Extensions.Logging;

namespace BloggerAgent.Infrastructure.Tooling.Context
{
    public class GetOrganizationContextTool : ILlmTool
    {
        private readonly IOrganizationRepository _organizationRepository;
        private readonly ILogger<GetOrganizationContextTool> _logger;

        public GetOrganizationContextTool(
            IOrganizationRepository organizationRepository,
            ILogger<GetOrganizationContextTool> logger)
        {
            _organizationRepository = organizationRepository;
            _logger = logger;
        }

        public string Name => "getOrganizationContext";

        public string Description =>
            "Retrieves the saved organization context using the platform-provided organization ID.";

        public Type ParameterType => typeof(GetOrganizationContextInput);

        public async Task<ToolResult> ExecuteAsync(object input)
        {
            _logger.LogInformation("Tool '{ToolName}' invoked.", Name);

            if (input is not GetOrganizationContextInput dto)
            {
                _logger.LogWarning(
                    "Invalid input provided to tool '{ToolName}'. Expected: {ExpectedType}, Received: {ReceivedType}",
                    Name,
                    nameof(GetOrganizationContextInput),
                    input?.GetType().Name ?? "null");

                return new ToolResult
                {
                    ToolName = Name,
                    Status = "error",
                    Output = "Invalid input: expected GetOrganizationContextInput"
                };
            }

            try
            {
                _logger.LogInformation("Fetching organization context for OrgId: {OrgId}", dto.OrganizationId);

                var companies = await _organizationRepository.GetAllAsync();

                if (companies == null || companies.Count == 0)
                {
                    _logger.LogInformation("No organization context found for OrgId: {OrgId}", dto.OrganizationId);

                    return new ToolResult
                    {
                        ToolName = Name,
                        Status = "not_found",
                        Output = new { found = false, organization = (object)null }
                    };
                }

                var company = companies.FirstOrDefault();
                _logger.LogInformation("Organization context found for OrgId: {OrgId}. Returning company: {CompanyName}", dto.OrganizationId, company?.Name);

                return new ToolResult
                {
                    ToolName = Name,
                    Status = "success",
                    Output = new
                    {
                        found = true,
                        organization = new
                        {
                            company.Name,
                            company.Overview,
                            company.Industry,
                            company.Website,
                            company.Tone,
                            company.TargetAudience
                        }
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred in tool '{ToolName}' for OrgId: {OrgId}", Name, dto.OrganizationId);

                return new ToolResult
                {
                    ToolName = Name,
                    Status = "error",
                    Output = "An unexpected error occurred while retrieving organization context."
                };
            }
        }
    }
}