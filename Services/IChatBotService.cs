using Entities;
using System.Threading.Tasks;

namespace Services
{
    public interface IChatBotService
    {
        Task<ChatBotResponse> AskQuestionAsync(ChatBotRequest request);
        Task<bool> UpdateDatabaseAsync();
    }
}
