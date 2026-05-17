using Entities;
using Repositories;
using System.Threading.Tasks;

namespace Services
{
    public class ChatBotService : IChatBotService
    {
        private readonly IChatBotRepository _chatBotRepository;

        public ChatBotService(IChatBotRepository chatBotRepository)
        {
            _chatBotRepository = chatBotRepository;
        }

        public async Task<ChatBotResponse> AskQuestionAsync(ChatBotRequest request)
        {
            // Add any business logic, validation, or logging here if necessary
            return await _chatBotRepository.AskQuestionAsync(request);
        }
    }
}
