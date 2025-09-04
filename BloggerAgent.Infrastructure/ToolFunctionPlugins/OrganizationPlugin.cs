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

        [KernelFunction("save_or_update_organization_context")]
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

       
    }
}
