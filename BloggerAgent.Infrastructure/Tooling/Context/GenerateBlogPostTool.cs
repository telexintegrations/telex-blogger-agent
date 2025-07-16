//using BloggerAgent.Application.Dtos;
//using BloggerAgent.Domain.IRepositories;
//using BloggerAgent.Infrastructure.Tooling;
//using Microsoft.Extensions.Logging;

//public class SaveOrganizationContextTool : ILlmTool
//{
//    private readonly IOrganizationRepository _companyService;
//    private readonly ILogger<SaveOrganizationContextTool> _logger;

//    public SaveOrganizationContextTool(
//        IOrganizationRepository companyService,
//        ILogger<SaveOrganizationContextTool> logger)
//    {
//        _companyService = companyService;
//        _logger = logger;
//    }

//    public string Name => "saveOrganizationContext";

//    public string Description =>
//        "Saves the organization's name, tone, target audience, website, and industry for later use during blog generation.";

//    public Type ParameterType => typeof(SaveOrganizationContextInput);

//    public async Task<object> ExecuteAsync(object input)
//    {
//        _logger.LogInformation("Executing tool: {ToolName}", Name);

//        if (input is not SaveOrganizationContextInput dto)
//        {
//            _logger.LogWarning("Invalid input type for tool {ToolName}. Received: {InputType}",
//                Name, input?.GetType().Name ?? "null");

//            throw new ArgumentException("Invalid input type for SaveOrganizationContextTool");
//        }

//        try
//        {
//            _logger.LogInformation("Saving organization context for: {OrgName}", dto.Name);

//            await _companyService.CreateCompanyAsync(dto);

//            _logger.LogInformation("Successfully saved organization context for: {OrgName}", dto.Name);

//            return new { success = true, message = "Organization context saved." };
//        }
//        catch (Exception ex)
//        {
//            _logger.LogError(ex, "Error saving organization context for: {OrgName}", dto.Name);
//            throw;
//        }
//    }
//}
