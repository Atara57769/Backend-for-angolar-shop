using Entities;
using Repositories;
using System.Threading.Tasks;

namespace Services
{
    public class ChatBotService : IChatBotService
    {
        private readonly IChatBotRepository _chatBotRepository;
        private readonly IProductRepository _productRepository;

        public ChatBotService(IChatBotRepository chatBotRepository, IProductRepository productRepository)
        {
            _chatBotRepository = chatBotRepository;
            _productRepository = productRepository;
        }

        public async Task<ChatBotResponse> AskQuestionAsync(ChatBotRequest request)
        {
            // Add any business logic, validation, or logging here if necessary
            return await _chatBotRepository.AskQuestionAsync(request);
        }

        private async Task<bool> SyncProductsAsync()
        {
            var products = await _productRepository.GetAllAvailableProductsAsync();
            return await _chatBotRepository.SyncProductsAsync(products);
        }

        public async Task<bool> UpdateDatabaseAsync()
        {
            // Fully synchronize the products before rebuilding the DB
            await SyncProductsAsync();
            return await _chatBotRepository.UpdateDatabaseAsync();
        }
    }
}
