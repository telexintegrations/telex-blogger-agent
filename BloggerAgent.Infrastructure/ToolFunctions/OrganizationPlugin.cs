using BloggerAgent.Domain.IRepositories;
using BloggerAgent.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloggerAgent.Infrastructure.ToolFunctions
{

    public class OrganizationPlugin
    {

        private readonly IServiceScopeFactory _scopeFactory;

        public OrganizationPlugin(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        [KernelFunction("save_organization_context")]
        [Description("Stores or creates a new organization profile used to generate personalized blog content.")]
        public async Task<string> SaveOrganizationAsync(
           string name, string overview, string industry,
           string website = null, string tone = null, string targetAudience = null)
        {
            using var scope = _scopeFactory.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<IOrganizationRepository>();

            var orgs = await repo.GetAllAsync();

            var existingOrg = orgs.FirstOrDefault();

            if (existingOrg != null)
            {
                existingOrg.Website = website ?? existingOrg.Website;
                existingOrg.Name = name ?? existingOrg.Name;
                existingOrg.Tone = tone ?? existingOrg.Tone;
                existingOrg.TargetAudience = targetAudience ?? existingOrg.TargetAudience;
                existingOrg.Overview = overview ?? existingOrg.Overview;
                existingOrg.UpdatedAt = DateTime.UtcNow;

                var result = await repo.UpdateAsync(existingOrg.Id, existingOrg);

                return result
                ? $"✅ Organisation '{existingOrg.Name}' updated successfully."
                : $"❌ Failed to update organization '{name}'.";
            }

            var org = new Company
            {
                Name = name,
                Overview = overview,
                Industry = industry,
                Website = website,
                Tone = tone,
                TargetAudience = targetAudience,
                UpdatedAt = DateTime.UtcNow
            };

            var success = await repo.CreateCompanyAsync(org);
            return success
                ? $"✅ Organization '{org.Name}' saved successfully."
                : $"❌ Failed to save organization '{name}'.";
        }

        [KernelFunction("Get_Date_Time")]
        public Task<string> GetDateTime()
        {
            return Task.FromResult(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
        }

        //[KernelFunction("update_organization_context")]
        //[Description("Updates a saved organization profile using its ID.")]
        //public async Task<string> UpdateOrganizationAsync(Company updatedCompany)
        //{
        //    using var scope = _scopeFactory.CreateScope();
        //    var repo = scope.ServiceProvider.GetRequiredService<IOrganizationRepository>();

        //    var success = await repo.UpdateCompanyAsync(updatedCompany);
        //    return success
        //        ? $"🔄 Organization '{updatedCompany.Name}' updated successfully."
        //        : $"⚠️ Failed to update organization.";
        //}

        //[KernelFunction("get_organization_context")]
        //[Description("Retrieves an organization context to be used in the blog post generation")]
        //public async Task<string> GetOrganizationContextAsync()
        //{
        //    using var scope = _scopeFactory.CreateScope();
        //    var repo = scope.ServiceProvider.GetRequiredService<IOrganizationRepository>();

        //    var orgs = await repo.GetAllAsync();
        //    if (orgs.Count == 0)
        //    {
        //        return "No company information recorded yet";
        //    }
        //    return string.Join(",", orgs.FirstOrDefault());

        //}
    }
}
