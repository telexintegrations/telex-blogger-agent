using TelexBloggerAgent.IRepositories;
using TelexBloggerAgent.IServices;
using TelexBloggerAgent.Models;

namespace TelexBloggerAgent.Services
{
    public class ConversationService : IConversationService
    {
        private readonly IConversationRepository _conversationRepository;

        public ConversationService(IConversationRepository conversationRepository)
        {
            _conversationRepository = conversationRepository;
        }

        public async Task<Conversation> StartConversationAsync(string userId, string role, string initialMessage)
        {
            var conversation = new Conversation
            {
                UserId = userId,
                Messages = new List<Message>()

            };

            var message = new Message { ConversationId = conversation.Id, Content = initialMessage, Role = role, Timestamp = DateTime.UtcNow };             

            conversation.Messages.Add(message);

            await _conversationRepository.CreateAsync(conversation);
            return conversation;
        }

        public async Task<Conversation?> GetConversationAsync(string conversationId)
        {
            return await _conversationRepository.GetByIdAsync(conversationId);
        }

        public async Task<Conversation> GetUserConversationsAsync(string userId)
        {
            return await _conversationRepository.GetConversationsByUserAsync(userId);
        }

        public async Task AddMessageAsync(string conversationId, string messageText, string role)
        {
            var message = new Message
            {
                ConversationId = conversationId,
                Content = messageText,
                Role = role,
                Timestamp = DateTime.UtcNow
            };

            await _conversationRepository.AddMessageAsync(conversationId, message);
        }
    }

}
