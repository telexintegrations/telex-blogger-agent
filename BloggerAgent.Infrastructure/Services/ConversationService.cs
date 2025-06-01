using BloggerAgent.Domain.IRepositories;
using BloggerAgent.Application.IServices;
using BloggerAgent.Domain.Models;

namespace BloggerAgent.Infrastructure.Services
{
    public class ConversationService : IConversationService
    {
        private readonly IConversationRepository _conversationRepository;

        public ConversationService(IConversationRepository conversationRepository)
        {
            _conversationRepository = conversationRepository;
        }

        //public async Task<Conversation> StartConversationAsync(string userId, string role, string initialMessage)
        //{
           

        //    var message = new Message 
        //    { 
        //        ConversationId = threadId ?? userId, 
        //        SourceId = userId, 
        //        Content = initialMessage, 
        //        Role = role, 
        //        Timestamp = DateTime.UtcNow 
        //    };             

            

        //    await _conversationRepository.CreateAsync(message);
        //    return message;
        //}

        //public async Task<Conversation?> GetConversationAsync(string conversationId)
        //{
        //    return await _conversationRepository.GetByIdAsync(conversationId);
        //}

        //public async Task<Conversation> GetUserConversationsAsync(string userId)
        //{
        //    return await _conversationRepository.GetConversationsByUserAsync(userId);
        //}

        //public async Task AddMessageAsync(string conversationId, string messageText, string role)
        //{
        //    var message = new Message
        //    {
        //        ConversationId = conversationId,
        //        Content = messageText,
        //        Role = role,
        //        Timestamp = DateTime.UtcNow
        //    };

        //    await _conversationRepository.AddMessageAsync(conversationId, message);
        //}
    }

}
