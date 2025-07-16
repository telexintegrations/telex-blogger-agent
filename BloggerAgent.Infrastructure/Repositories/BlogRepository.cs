using BloggerAgent.Domain.Commons;
using BloggerAgent.Domain.Data;
using BloggerAgent.Domain.IRepositories;
using BloggerAgent.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloggerAgent.Infrastructure.Repositories
{
    public class BlogRepository : MongoRepository<Blog>, IBlogRepository
    {

        private readonly IMongoRepository<Blog> _blogRepository;

        public BlogRepository(IMongoRepository<Blog> blogRepository, DbContext context) : base(context)
        {
            _blogRepository = blogRepository;
        }


        public async Task<bool> AddBlogAsync(Blog blogPost)
        {
            if (blogPost == null)
            {
                throw new ArgumentException("Blog cannot be null");
            }
            return await _blogRepository.CreateAsync(blogPost);
        }

        public async Task<bool> UpdateBlogAsync(Blog company, string orgId)
        {
            if (company == null)
            {
                throw new ArgumentException("Company cannot be null or have an empty name.");
            }

            var companies = await FilterByFieldAsync("tag", CollectionType.Blog);

            if (companies == null) return false;

            var existingCompany = companies.FirstOrDefault();

            if (existingCompany == null)
            {
                throw new ArgumentException("Blog with orgId {orgId} does not exist.", orgId);
            }

            existingCompany.Title = company.Title;
            existingCompany.Content = company.Content;
            existingCompany.Keywords = company.Keywords;
            existingCompany.UpdatedAt = company.UpdatedAt;

            return await _blogRepository.UpdateAsync(existingCompany.Id, company);
        }

    }
}
