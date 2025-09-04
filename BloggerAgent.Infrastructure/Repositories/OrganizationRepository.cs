using BloggerAgent.Domain.Commons.constants;
using BloggerAgent.Domain.Data;
using BloggerAgent.Domain.DomainHelper;
using BloggerAgent.Domain.IRepositories;
using BloggerAgent.Domain.Models;
using BloggerAgent.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloggerAgent.Domain.Repositories
{
    public class OrganizationRepository : TelexRepositoryBase<Company>, IOrganizationRepository
    {
        private readonly ITelexRepository<Company> _companyRepository;

        public OrganizationRepository(ITelexRepository<Company> companyRepository, DbContext context): base(context) 
        {
            _companyRepository = companyRepository;
        }

        public async Task<bool> CreateCompanyAsync(Company company)
        {
            if (company == null || string.IsNullOrEmpty(company.Name))
            {
                throw new ArgumentException("Company cannot be null or have an empty name.");
            }
            return await _companyRepository.CreateAsync(company);
        }

        public async Task<bool> UpdateCompanyAsync(Company company)
        {
            if (company == null)
            {
                throw new ArgumentException("Company cannot be null or have an empty name.");
            }

            var companies = await _companyRepository.FilterByFieldAsync("tag", CollectionType.Company);

            if (companies == null)
            {
                throw new ArgumentException("Company with orgId {OrgId} does not exist.", OrgId);
            }

            var existingCompany = companies.FirstOrDefault();

            existingCompany.Name = company.Name ?? existingCompany.Name;
            existingCompany.Overview = company.Overview ??  existingCompany.Overview;
            existingCompany.Industry = company.Industry ?? existingCompany.Industry;
            existingCompany.Tone = company.Tone ?? existingCompany.Tone;
            existingCompany.TargetAudience = company.TargetAudience ?? existingCompany.TargetAudience;
            existingCompany.Website = company.Website ?? existingCompany.Website;
            existingCompany.UpdatedAt = company.UpdatedAt;

            return await _companyRepository.UpdateAsync(existingCompany.Id!, company);
        }

    }
}
