using Microsoft.Extensions.Options;
using MongoDB.Driver;
using BloggerAgent.Domain.Data;
using BloggerAgent.Application.Helpers;
using BloggerAgent.Domain.IRepositories;
using BloggerAgent.Domain.Models;
using BloggerAgent.Application.Dtos;
using BloggerAgent.Domain.Commons.Gemini;
using BloggerAgent.Domain.Commons;
using DnsClient.Internal;
using Microsoft.Extensions.Logging;
using BloggerAgent.Domain.Commons.DataEntities;

namespace BloggerAgent.Infrastructure.Repositories
{
    public class ConversationRepository : TelexRepository<Message>, IConversationRepository
    {
        private readonly DbContext _context;
        private readonly ITelexRepository<Message> _repository;
        private readonly ILogger<ConversationRepository> _logger;

        public ConversationRepository(DbContext context, ILogger<ConversationRepository> logger, ITelexRepository<Message> repository) : base(context)
        {
            _context = context;
            _repository = repository;
            _logger = logger;
        }

        public async Task<Message> GetConversationsByUserAsync(string userId)
        {
            return await _repository.GetByIdAsync(userId);
        }

        public async Task<List<TelexChatMessage>> GetMessagesAsync(string contextId)
        {
            var conversations = await _repository.GetAllAsync();
            if (conversations == null)
            {
                throw new Exception("Failed to retrieve messages");
            }

            return conversations
                .Where(c => c.ContextId == contextId)
                .Take(10)
                .OrderBy(m => m.Timestamp)
                .Select(c => new TelexChatMessage()
                {
                    Role = c.Role,
                    Content =  c.Content 
                }).ToList();
        }

        public async Task<bool> AddNewMessagesAsync(string message, TaskContext blogDto, string role)
        {

            var newMessage = new Message
            {
                UserId = blogDto.UserId,
                Content = message,
                TaskId = blogDto.TaskId,
                ContextId = blogDto.ContextId,
                Role = role
            };

            bool isAdded = await _repository.CreateAsync(newMessage);

            if (!isAdded)

            {
                _logger.LogInformation($"Failed to add {newMessage.Role} message to database");
                return false;
            }
            _logger.LogInformation($"Message for {newMessage.Role} added successfully to database");

            return true;

        }


        //public async Task<Company> AddCompanyAsync(Company company)
        //{
        //    if (company.Id == null)
        //        throw new ArgumentNullException();

        //    var existingCompany = await _companyRepository.GetByIdAsync(company.Id);

        //    if (existingCompany != null)
        //    {
        //        throw new Exception("Company already exists");
        //    }

        //    var newCompany = new Company
        //    {
        //        Id = company.Id,
        //        Name = company.Name,
        //        Tone = company.Tone,
        //        TargetAudience = company.TargetAudience,
        //        Overview = company.Overview,
        //        Industry = company.Industry,
        //    };

        //    /*var channelId = company.Users.FirstOrDefault().Id;

        //    var user = await _userService.GetUserAsync(channelId);

        //    if (user != null)
        //    {
        //        throw new DuplicateNameException();
        //    }
        //    var newUser = new User { Id = channelId };

        //    newCompany.Users.Add(newUser);

        //    try
        //    {
        //    }
        //    catch (Exception)
        //    {
        //        // Rollback: Delete the company if user creation fails
        //        await _userRepository.DeleteAsync(newUser.Id);
        //        throw;
        //    }*/

        //    // Register the company’s communication channel as a user
        //    await _companyRepository.CreateAsync(newCompany);

        //    return newCompany;
        //}

        //public async Task<Company> UpdateCompanyAsync(Company company)
        //{
        //    if (company.Id == null)
        //        throw new ArgumentNullException();

        //    var companyDoc = await _companyRepository.GetByIdAsync(company.Id);

        //    if (companyDoc == null)
        //    {
        //        throw new Exception("Company not found exists");
        //    }

        //    var existingCompany = companyDoc.Data;

        //    existingCompany.Id = company.Id ?? existingCompany.Id;
        //    existingCompany.Name = company.Name ?? existingCompany.Name;
        //    existingCompany.Tone = company.Tone ?? existingCompany.Tone;
        //    existingCompany.TargetAudience = company.TargetAudience ?? existingCompany.TargetAudience;
        //    existingCompany.Overview = company.Overview ?? existingCompany.Overview;
        //    existingCompany.Industry = company.Industry ?? existingCompany.Industry;



        //    // Register the company’s communication channel as a user
        //    await _companyRepository.UpdateAsync("", existingCompany);

        //    return existingCompany;
        //}
    }
}

