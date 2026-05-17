using Entities;
using System.Threading.Tasks;

namespace Repositories
{
    public interface IChatBotRepository
    {
        Task<ChatBotResponse> AskQuestionAsync(ChatBotRequest request);
    }
}
